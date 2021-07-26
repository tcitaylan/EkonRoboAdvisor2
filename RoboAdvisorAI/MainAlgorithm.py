import os
from prediction.Training import *
from prediction.Predicter import *
from Backtest import *
from categorization.Fuzzy import *
from data import *
from allocation.BlackLitterman import *
from categorization.Fuzzy import *
from categorization.SpecialBaskets import *
import time
import datetime as d 
import schedule as s
from Backtest import *
import time
from Notifier import *
from Settings import *
from CreatePDF import *





#Son güne ait veriler ile algoritma çalıştırılır.
def CalculateTodaysResults(start, end):
    mailitem = ""
    basketnames = [] #bülten için
    basketsfpdf = [] #bülten için
    datax = [[]] #bülten paket
    datanames = [] #bülten isim
    datavalues = [] #bülten data

       

    categories = Preprocess.GetCategories()

    result = []

    df = Preprocess.PrepareDataframe("*", start, end)

    print("Data prepare completed")

    deviations = Preprocess.StandartDeviation(df)
   
    print("Risk calculations completed")

    templateBasketIds = {}

    for categoryIndex, category in enumerate(categories): 
        baskets = []
        basketNames = []
        resultArray = []
        basketids = []

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
            result = {}
            filteredsym = []

            for sym in basket:
                sym = sym.replace('Asset','')
                filteredsym.append(sym)

            predictions = PredictSymbolsByDate(filteredsym, start, str(df.tail(1).index[0]))

            print("Neural network predictions completed")

            basketDf = df.copy()

            for column in basketDf.columns:
                id = column.replace("Asset","")
                if column not in basket:
                    del basketDf[column]

            basketPredictions = []
          
            for p in predictions:
                basketPredictions.append([p[0],p[1],p[2][len(p[2])-1]])

            min = 0
            for p in predictions:
                if len(p[2]) < min or min == 0:
                    min = len(p[2])
            basketDf = basketDf.iloc[-min:]

            try:
                if len(basket) > 0: #Basket üretilen 
                   weightsRev = []
                   weights = Allocate(basketDf, basketPredictions)
                        
                   [weights.pop(key) for key in list(weights.keys()) if weights[key] == 0]
                   result = weights

                print(result)
                templateBasketId = DAL.insert_daily_data(result, category, basketNames[basketIndex])                

                createdBasket = DAL.getTemplateBasket(templateBasketId)
                
                
                mailitem += "\n" + createdBasket[0][0] + "\n\n" 
                basketnames.append(createdBasket[0][0]) #bülten basketname
                datax = [[]]    #bülten paket
                datanames = []  #bülten isim
                datavalues = [] #bülten data
                totalperc = 0
                
                for asset in createdBasket:
                    mailitem += " - " + str(asset[1]) + " : %" + str(round(asset[2]*100,2)) + "\n"                    
                    datanames.append(str(asset[1]))                    
                    datavalues.append(str(round(asset[2]*100,2)))  
                     
                    totalperc += round(asset[2]*100,2)
                if totalperc < 100:
                    mailitem += " - NAKİT : %" + str(100 - totalperc) + "\n"
                    datanames.append("NAKİT")
                    rounded_csh = round(100 - totalperc) 
                    datavalues.append(str(rounded_csh))
                datax[0] = [datanames, datavalues]
                
                basketsfpdf.append(datax)

                resultArray.append(weights)
                basketids.append(templateBasketId)

            except Exception as e:
                print("Error occured while creating basket " + str(e))
        
        templateBasketIds[category[1]] = basketids

    create_html(basketsfpdf, basketnames)
    print("Optimization completed for category : " + str(category[0]))
    sendmail(EmailRecipents, mailitem)
    CalculateBacktest("2019-01-01", str(d.date.today()), templateBasketIds)

    print("Backtests updated with new data.")

    return result

#Yapay sinir ağı tahminlemesi yapılır.
def GetPredictions(start, end):

    predictions = PredictAllSymbolsByDate(s8tart,end)
    predictions = OrderByPrediction(predictions)

    return predictions

CalculateTodaysResults("2019-01-01", str(d.date.today()))     

mins = 0
todayCalculated = False
inProgress = False

while True:
    now = d.datetime.now()

    weekDay = now.weekday()

    if weekDay == 5 or weekDay == 6:
        time.sleep(60)
        continue

    inProgress = True

    startTime = d.datetime.now() - d.timedelta(days=1.5 * 365)

    if now.hour == 20 and todayCalculated == False and inProgress != False:
        CalculateTodaysResults(str(startTime), str(d.date.today()))
        todayCalculated = True
    if now.hour > 20:
        todayCalculated = False

    if now.hour == 17 and now.minute > 15 and todayCalculated == False and inProgress != False:
        CalculateTodaysResults(str(startTime), str(d.date.today()))
        todayCalculated = True
    if now.hour == 18:
        todayCalculated = False

    if now.hour == 12 and now.minute > 45 and todayCalculated == False and inProgress != False:
        CalculateTodaysResults(str(startTime), str(d.date.today()))
        todayCalculated = True
    if now.hour == 13:
        todayCalculated = False

    inProgress = False
    time.sleep(60)

