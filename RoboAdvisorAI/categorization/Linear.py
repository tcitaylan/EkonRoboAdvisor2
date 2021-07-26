import pandas as pd
import os
from sklearn import preprocessing, metrics
from sklearn.preprocessing import MinMaxScaler
from collections import deque
import random
import numpy as np
import traceback
from data.DataAccessLayer import DataAccessLayer as DAL
import statistics
from Settings import *

#İlgili kategoriye ait oluşan sepetleri döndürür.
def GetLinearBaskets(df, deviations, category, categoryCount):
    baskets = []

    maxDev, minDev = GetMaxDeviation(deviations, category, categoryCount)

    basketSymbols = []
    riskFreeSymbols = []

    for column in df.columns:

        if column == "AssetNakit":
            continue

        id = column.replace("Asset","")
        deviation = deviations[0]
        for dev in deviations:
            if(dev["id"] == id):
                deviation = dev
                break
  
        if float(deviation["value"]) <= maxDev and float(deviation["value"]) >= minDev and np.isnan(float(deviation["value"])) == False:
        #if float(deviation["value"]) <= maxDev and np.isnan(float(deviation["value"])) == False:
            basketSymbols.append(column)
        elif deviation["value"] == deviations[0]["value"] or deviation["value"] == deviations[1]["value"] or deviation["value"] == deviations[2]["value"]:
            riskFreeSymbols.append(column)

    if(projectType == "hisse"):
        riskFreeSymbols = []

    createdBasket = []

    #limit = random.randint(8,12)
    
    dividedCount = len(basketSymbols) / 2

    if len(basketSymbols) > 0:
        for b_index in range(2):
            for iterator in range(int(dividedCount * b_index),int(dividedCount * (b_index+1))):     
                #randInt = random.randint(0,len(basketSymbols)-1)
                randomSymbol = basketSymbols[iterator]
                if(randomSymbol not in createdBasket):
                    createdBasket.append(randomSymbol)
                #del basketSymbols[randInt]

            createdBasket = createdBasket + riskFreeSymbols
            baskets.append(createdBasket)
            createdBasket = []


    #baskets.append(basketSymbols);
    
    return baskets

#İlgili kategoriye ait maksimum risk değerini döndürür
def GetMaxDeviation(deviations, category, categoryCount):
    riskList = []

    for x in deviations:
        riskList.append(float(x["value"]))

    riskList = np.array(riskList)

    riskList = riskList[~np.isnan(riskList)]

    riskList.sort()

    step = int(len(deviations) / categoryCount)

    riskCat = riskList[step*(category):step*(category+1)]

    return max(riskCat), min(riskCat)