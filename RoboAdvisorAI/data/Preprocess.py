import pandas as pd
import os
from sklearn import preprocessing, metrics
from sklearn.preprocessing import MinMaxScaler
from collections import deque
import random
import numpy as np
import traceback
import tulipy as ti
from data.DataAccessLayer import DataAccessLayer as DAL
import statistics
from data.DataService import DataService as DS
from Settings import *
#-------------------NEURAL NETWORK----------------------#

def CreateDataframe(symbol):
    data = None

    if projectType == "hisse":
        data = DS.getHistoricalData(symbol[1])
    else:
        data = DAL.getHistoricalData(symbol[0])

    df = pd.DataFrame() 

    rows_as_tuples = [tuple(x) for x in data]
    closeList = np.array([item[1] for item in rows_as_tuples])

    indicatorData, indicatorName = PrepareIndicatorData(closeList);

    for i in range(0, len(indicatorData)):
        df[indicatorName[i]] = indicatorData[i]

    closeList = closeList[1:]
    closeList = np.append(closeList,0)

    df["TARGET"] = closeList

    df = df[50:]

    return df

def CreateDataframeByDate(symbol, start, end):
    data = None

    if projectType == "hisse":
        data = DS.getHistoricalDataDate(symbol[1], start, end)
    else:
        data = DAL.getHistoricalDataDate(symbol[0], start, end)

    df = pd.DataFrame() 

    rows_as_tuples = [tuple(x) for x in data]
    closeList = np.array([item[1] for item in rows_as_tuples])

    indicatorData, indicatorName = PrepareIndicatorData(closeList);

    for i in range(0, len(indicatorData)):
        df[indicatorName[i]] = indicatorData[i]

    closeList = closeList[1:]
    closeList = np.append(closeList,0)

    df["TARGET"] = closeList

    df = df[50:]

    return df

def PrepareIndicatorData(DATA):
    ma = ti.sma(DATA, period=7)
    ma2 = ti.sma(DATA, period=25)
    ma3 = ti.sma(DATA, period=50)
    rsi = ti.rsi(DATA, 14)
    linreg = ti.linreg(DATA, 14)
    mom = ti.mom(DATA, 14)
    ema = ti.ema(DATA, 14)

    inds = [ma,ma2,ma3,rsi,linreg,mom,ema];
    indsname = ["ma","ma2","ma3","rsi","linreg","mom","ema"];
    
    #inds = [ma,ma2,ma3]
    #indsname = ["ma","ma2","ma3"]

    for idx in range(len(inds)):
        arr = np.zeros(len(DATA)-len(inds[idx]))
        inds[idx] = np.insert(inds[idx],0,arr)

    return inds, indsname

def NormalizeDataFrame(df, SEQ_LEN):

    ##HER SÜTUNUN KENDİ İÇİNDE NORMALİZE EDİLMESİ GEREKLİ BU HATALI
    #for col in df.columns:
    #   if col.find("TARGET") == -1:
    #         df[col] = df[col].pct_change()  
    #         df = df.replace([np.inf, -np.inf], np.nan)
    #         df.dropna(inplace = True)
    #         df[col] = preprocessing.scale(df[col].values)
    #         df.dropna(inplace=True)


    #inverse = scaler.inverse_transform(normalized)

    normalized = TransformData(df.values)

    sequential_data = []
    prev_days = deque(maxlen=SEQ_LEN)

    #print(df.head())
    #print(normalized)

    for i in normalized:  
       prev_days.append([n for n in i[:-1]]) 
       if len(prev_days) == SEQ_LEN:  
          sequential_data.append([np.array(prev_days), i[-1]])  

    #random.shuffle(sequential_data) 

    x = []
    y = []

    for seq, target in sequential_data:  
        x.append(seq)  
        y.append(target)

    return np.array(x), np.array(y)

def TransformData(vals):
    scaler = MinMaxScaler(feature_range=(0,1))
    normalized = scaler.fit_transform(vals)
    return normalized

def InverseTransformData(vals):
    scaler = MinMaxScaler(feature_range=(0,1))
    scaler.fit(vals)
    inversed = scaler.inverse_transform(vals)
    return inversed

#-------------------NEURAL NETWORK----------------------#



#-------------------MAIN ALGORITHM--------------------------#

def PrepareDataframe(idList, startDate, endDate):
    symbols = None

    if(idList == "*"):
        idList = []
        namelist = []

        symbols = DAL.getSymbols()

        for sym in symbols:
            if projectType == "hisse":
                idList.append(sym[0])
                namelist.append(sym[1])
            else:
                idList.append(sym[0])

    data = None
    if projectType == "hisse":
        data = DS.getSymbolsDataByDate(namelist,idList,startDate,endDate)
    else:
        data = DAL.getSymbolsDataByDate(idList,startDate,endDate)

    df = pd.DataFrame() 

    rows_as_tuples = [tuple(x) for x in data]

    idList = np.array([item[0] for item in rows_as_tuples])
    dateList = np.array([item[1] for item in rows_as_tuples])
    idList = list(set(idList))
    dateList = list(set(dateList))

    dateList.sort() 
    
    headerList = ["Date"]
    for x in idList:
        headerList.append('Asset' + str(x))

    rows = []

    for date in dateList:
        row = [date]
        row = np.pad(row, (0, len(headerList)-1), 'constant')
        rows.append(row)

    df = pd.DataFrame(rows)    

    df = df.set_axis(headerList, axis=1, inplace=False)

    df = df.set_index("Date")

    for value in data:
        df["Asset"+str(value[0])][value[1]] = value[2]

    df = df.replace("0", 0)

    print(df.head())

    df = df[5:]

    for col in df.columns:
        if(df[col].values[0] == 0):
            del df[col]

    for column in df.columns:
        vals = df[column].values
        for i,x in enumerate(vals):
            if(i < len(vals) - 2 and i > 1 and vals[i] != 0 and  vals[i-1] != 0):
                if abs(vals[i] - vals[i-1]) / vals[i] > 0.35:
                    print("Asset removed, one day change > %25 : "+ column)
                    del df[column]
                    break
            if vals[i] == 0 and i > 0:
                df[column][i] = df[column][i-1]

    starter = 100
    cashVals = []

    for vals in df[df.columns[0]].values:
        starter *= (1+0.0003)
        cashVals.append(starter)

    df["AssetNakit"] = cashVals

    return df

def GetCategories():
    categories = []

    categories = DAL.getRiskCategories()

    categories = [tuple(x) for x in categories]
    res=[]
    for index,cat in enumerate(categories):
        res.append([str(index+1), cat[0]])

    return res

#-------------------MAIN ALGORITHM--------------------------#



#---------------------FUNCTIONS--------------------------#

def StandartDeviation(symbolsDataframe):  
    results = []

    df = symbolsDataframe.copy()
    df = df.pct_change()
    #df.dropna(inplace = True)

    for column in symbolsDataframe:
        try:
            symdata = df[column].values        
            symdata[np.isinf(symdata)] = np.NaN
            symdata = symdata[~np.isnan(symdata)]

            results.append({
                "name": column,
                "value":"%.16f" % (statistics.stdev(symdata) * statistics.stdev(symdata)),
                "id":column.split("Asset")[1]
            })
        except:
            continue

    return sorted(results, key=lambda k: float(k["value"]))

#---------------------FUNCTIONS--------------------------#