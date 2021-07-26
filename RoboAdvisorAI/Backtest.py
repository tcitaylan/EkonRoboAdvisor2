import os
from collections import deque
import numpy as np
import matplotlib.pylab as plt
import pandas as pd
import os.path
import datetime 
import statistics
import itertools
import matplotlib
import csv
from prediction.Training import *
from prediction.Predicter import *
from Backtest import *
from categorization.Fuzzy import *
from data import *
from allocation.BlackLitterman import *
from categorization.SpecialBaskets import *
import time
import datetime
import schedule as s
from categorization.Linear import *
from data.DataAccessLayer import DataAccessLayer as DAL
from Settings import *
import json

#Ana backtest metotu
def CalculateBacktest(start, end, templateIds=None):
    
    categories = Preprocess.GetCategories()

    resultArray = []

    df = Preprocess.PrepareDataframe("*", start, end)

    deviations = Preprocess.StandartDeviation(df)
   
    deviationCoefs = []

    print("Risk calculations completed")

    for categoryIndex, category in enumerate(categories):
        #category = categories[2]
        #categoryIndex = 2

        baskets = []
        basketNames = []

        if("fuzzy" in categorizationTypes):
            membershipFuncs = CreateFuzzyClasses(deviations, len(categories))
            print("Fuzzy classes created")  
            fuzzybaskets = GetFuzzyBaskets(df, deviations, category, membershipFuncs)
            for i,x in enumerate(linearbaskets):
                basketNames.append("Bulanık Kategori " + str(categoryIndex + 1) + " (Sepet " + str(i + 1) + ")")
            baskets += fuzzybaskets
            print("Fuzzy baskets created")  
        if("linear" in categorizationTypes):
            linearbaskets = GetLinearBaskets(df, deviations, categoryIndex, len(categories))        
            for i,x in enumerate(linearbaskets):
                basketNames.append("Kategori " + str(categoryIndex + 1) + " (Sepet " + str(i + 1) + ")")
            baskets += linearbaskets
            print("Linear baskets created")  
        if("specialbaskets" in categorizationTypes):
            names, specialbaskets = GetSpecialBaskets(df, category)
            baskets += specialbaskets
            basketNames += names
            print("Special baskets created")  

        for basketIndex, basket in enumerate(baskets):
            filteredsym = []

            for sym in basket:
                sym = sym.replace('Asset','')
                filteredsym.append(sym)

            predictions = PredictSymbolsByDate(filteredsym, start, end)

            print("Neural network predictions completed")

            basketDf = df.copy()

            for column in basketDf.columns:
                id = column.replace("Asset","")
                if column not in basket and column != "AssetNakit":
                    del basketDf[column]

            result = RealBacktest(start, end, basketDf, predictions, basketNames[basketIndex])
            resultArray.append(result)

            if(templateIds is not None):    
                backtestLst = []

                for i,balance in enumerate(result[0]):
                    backtestLst.append({"Balance":balance, "LatestProfit":result[1][i], "Date":result[3][i].strftime("%Y-%m-%d")})

                DAL.insertBacktest(json.dumps(backtestLst), templateIds[category[1]][basketIndex])

        print("Backtest completed for category : " + str(categoryIndex + 1))

    return resultArray

#Backtest yapar
def RealBacktest(startDate, endDate, df, predictions, categoryName):
    min = 0

    for p in predictions:
        if len(p[2]) < min or min == 0:
            min = len(p[2])

    df = df.iloc[-min:]

    sd = startDate.split('-')
    ed = endDate.split('-')

    backtestEnd = date(int(ed[0]), int(ed[1]), int(ed[2]))
    current_date = date(int(sd[0]), int(sd[1]), int(sd[2]))

    delta = timedelta(days=1)

    counter = 0

    lastDate = ""
    
    dateArray = []
    basketArray = []
    totalBalanceArray = []
    profitArray = []
    currentCosts = []
    nextBasketDate = current_date
  
    symbols = DAL.getSymbols()
    symbols = [tuple(x) for x in symbols]

    buyValors = {}
    sellValors = {}

    for sym in symbols:
        if sym[11] == None:
            buyValors["Asset" + str(sym[0])] = 0
        else:
            buyValors["Asset" + str(sym[0])] = sym[11]

        if sym[12] == None:
            sellValors["Asset" + str(sym[0])] = 0
        else:
            sellValors["Asset" + str(sym[0])] = sym[12]

    buyValors["AssetNakit"] = 0
    sellValors["AssetNakit"] = 0

    while current_date < backtestEnd:

        current_date += delta

        sub_df = df[(df.index <= current_date.strftime("%Y-%m-%d"))]   

        if(sub_df.tail(1).index <= lastDate or sub_df.shape[0] == 0):
            continue

        lastDate = sub_df.tail(1).index

        preds = []

        for p in predictions:
            preds.append([p[0],p[1],p[2][counter]])
           
        counter += 1

        if (current_date >= nextBasketDate):
            nextBasketDate = current_date + timedelta(days=minRebalanceDay)

            weights = Allocate(sub_df, preds)          

            basket = {"content":[], "date": current_date.strftime("%Y-%m-%d")}
            for x in weights:
                stringName = ""
                for sym in symbols :
                    if str(sym[0]) == str(x.replace('Asset','')):
                        stringName = sym[1]
                        break

                basket["content"].append({"fundid":str(x.replace('Asset','')), "perc":weights[x], "name": stringName })
        elif len(basketArray) == 0:
            continue
        else:
            basket = basketArray[len(basketArray) - 1]
      
        currentBalance = 0
        currentProfit = 0

        if len(currentCosts) == 0 :
            for fund in basket["content"]:
                sym = 'Asset' + str(fund['fundid'])
                valor = buyValors[sym]
                price = GetValorPrice(df,sym,valor,current_date, backtestEnd)
                if price > 0 :
                    lot = (10000 * fund["perc"]) / price
                    currentCosts.append({"id":fund["fundid"],"lot":lot, "price": price})
                    currentBalance += (lot * price)  
                else :
                    lot = prevBalance * fund["perc"]
                    currentBalance += (prevBalance * fund["perc"]) 
                    currentCosts.append({"id":0,"lot":lot, "price": price}) 
        else :
            for index, fund in enumerate(currentCosts):
                sym = 'Asset' + str(fund['id'])
                valor = sellValors[sym]
                price = GetValorPrice(df,sym,valor,current_date, backtestEnd)
                if(price == 0):
                    price = fund["price"]
                currentProfit += (price - fund["price"]) * fund["lot"]
                lot = currentCosts[index]["lot"]
                fundid = currentCosts[index]["id"]
                cost = currentCosts[index]["price"]

            currentBalance += totalBalanceArray[len(totalBalanceArray) - 1] + currentProfit 

            currentCosts = []
            for fund in basket["content"]:
                sym = 'Asset' + str(fund['fundid'])
                valor = buyValors[sym]
                price = GetValorPrice(df,sym,valor,current_date, backtestEnd)
                if price > 0 :
                    lot = (currentBalance * fund["perc"]) / price
                    currentCosts.append({"id":fund["fundid"],"lot":lot, "price": price})

        profitArray.append(currentProfit)
        totalBalanceArray.append(currentBalance)
        basketArray.append(basket)
        dateArray.append(current_date)

    CreateBacktestResultFiles(str(categoryName), dateArray, profitArray, totalBalanceArray, basketArray)

    return totalBalanceArray, profitArray, basketArray, dateArray

def GetValorPrice(df, sym, valor, nowdate, backtestEnd):
    price = df.loc[str(nowdate)][sym]
    
    delta = timedelta(days=1)

    counter = 0

    while nowdate < backtestEnd:
        if nowdate in df.index:
            price = df.loc[str(nowdate)][sym]
            if(counter >= valor):
                break
        counter+=1
        nowdate += delta


    return price

#Verilen risk kategorisinde rasgele oluşturulmuş bir sepet ile backtest yapar.
def RandomBacktest(riskCategory, categoryCount, startDate, endDate):
    df = Preprocess.PrepareDataframe("*", startDate, endDate)

    devs = Preprocess.StandartDeviation(df)

    categoryStep = len(devs) / categoryCount
    sliced = list(itertools.islice(devs, int((riskCategory * categoryStep) - categoryStep), int(riskCategory * categoryStep)))
    print(sliced)

    values = [0.0, 1] + list(np.random.uniform(low=0.0,high=1,size=len(sliced) - 1))
    values.sort()
    weights = [values[i + 1] - values[i] for i in range(len(sliced))]

    cont = []

    for index,fund in enumerate(sliced):
        cont.append({"fundid":fund["id"], "perc": round(weights[index],3), "name":fund["name"]})

    sampleBaskets = [{"date":startDate,"content":cont }]

    totalBalanceArray, profitArray = MakeBackest(sampleBaskets, 10000, df, "RANDOM CATEGORY " + str(riskCategory))

    return totalBalanceArray, profitArray

#Backtest sonucunu dosyaya yazdırır.
def CreateBacktestResultFiles(backtestName, dateArray, profitArray, totalBalanceArray, basketArray):
    dates = []
    for row in dateArray:
        dates.append(matplotlib.dates.date2num(row))

    plt.figure(figsize=(10,6))
    plt.plot(dates, totalBalanceArray, color='blue', label='Total Balance')
    plt.title(backtestName)
    plt.xlabel('Date')
    plt.ylabel('Total Balance')
    plt.legend()
    ax = plt.gca()
    ax.xaxis.set_major_locator(matplotlib.dates.MonthLocator())
    ax.xaxis.set_major_formatter(matplotlib.dates.DateFormatter('%m-%Y'))
    plt.xticks(rotation=45)
    #plt.show()
    plt.savefig(savedBacktestPath + "\{}.png".format(str(datetime.datetime.now()).split(' ')[0] + "-" + backtestName))
    plt.close()

    plt.figure(figsize=(10,6))
    plt.plot(dates, profitArray, color='red', label='Profit')
    plt.title(backtestName)
    plt.xlabel('Date')
    plt.ylabel('Profit')
    plt.legend()
    ax = plt.gca()
    ax.xaxis.set_major_locator(matplotlib.dates.MonthLocator())
    ax.xaxis.set_major_formatter(matplotlib.dates.DateFormatter('%m-%Y'))
    plt.xticks(rotation=45)
    #plt.show()
    plt.savefig(savedBacktestPath + "\{}.png".format(str(datetime.datetime.now()).split(' ')[0] + "-" + backtestName + "-2"))
    plt.close()

    with open(savedBacktestPath + "\{}.csv".format(str(datetime.datetime.now()).split(' ')[0] + "-" + backtestName), 'w', newline='', encoding='utf-8') as file:
        writer = csv.writer(file, delimiter=';')
        row_list = [["Tarih","Sepet İçeriği","Toplam Bakiye"]]
        for index, bskt in enumerate(basketArray):
            contentString = ""
            for fnd in bskt["content"]:
                contentString += fnd["name"] + "(" + str(fnd["fundid"]) + ")" + " : " + str(fnd["perc"]) + "  ,  "
            row_list.append([dateArray[index], contentString, totalBalanceArray[index]])
        writer.writerows(row_list)

