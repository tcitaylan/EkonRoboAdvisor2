import sys
import numpy as np
import matplotlib.pyplot as plt
import skfuzzy as fuzz
from skfuzzy import control as ctrl
import operator
import pandas as pd
from numpy import linalg
import statistics
import random 

#Verilen data ve sınıf sayısına göre kategorileri oluşturur.
def CreateFuzzyClasses(riskData, classCount):      
    classes = []

    memberShips = GetMemberships(riskData, classCount)

    riskValues = np.array([row["value"] for row in riskData]).astype(np.float)

    for x in range(classCount):
        if(len(memberShips) > x):
            if(len(memberShips[x]) == 3):
                classes.append(fuzz.trimf(riskValues , memberShips[x]))
            elif(len(memberShips[x]) == 4):
                classes.append(fuzz.trapmf(riskValues , memberShips[x]))

    return classes

#Fuzzy sınıflarına ait üyelik fonksiyonlarını döndürür.
def GetMemberships(riskData, classCount): 
    result = [
        [0,0,.0000035], 
        [.0000035,0.000005,0.00001,0.000015], 
        [.00001,0.000019,0.00005,0.000052],
        [0.000015,0.000093,0.0001990], 
        [0.00000,.000095,.0001,.00021]
       ]

    result = []

    step = len(riskData) / classCount

    for i in range(0,step):
        result

    return result

#Verilen varlık risk datasında her sınıfa ait üyelik değerlerini bulur.
def CategorizeSymbol(riskData, membershipFuncs, symbol_in):
    memberships = []
    
    riskValues = np.array([row["value"] for row in riskData]).astype(np.float)

    for func in membershipFuncs:
        result = fuzz.interp_membership(riskValues, func, symbol_in)  
        memberships.append(result)

    assetCategories = []

    for index, item in enumerate(memberships):
        if item > 0:
            assetCategories.append(index)

    return assetCategories

#Üyelik fonksiyonları grafiği çizer
def PlotFuzzyChart(riskData, classes):
    riskValues = np.array([row["value"] for row in riskData]).astype(np.float)

    fig, (ax0) = plt.subplots(nrows=1, figsize=(8, 9))

    colors = ['blue', 'green', 'red', 'yellow', 'orange']

    for index, cs in enumerate(classes):
        ax0.plot(riskValues, cs, colors[index], linewidth=1.5, label='Cat'+str(index))

    ax0.set_title('Risk Categories')
    ax0.legend()

    plt.tight_layout()
    plt.show()

#İlgili kategoriye ait oluşan sepetleri döndürür.
def GetFuzzyBaskets(df, deviations, category, membershipFuncs):
    baskets = []

    basketSymbols = []
    for column in df.columns:
        id = column.replace("Asset","")
        deviation = deviations[0]
        for dev in deviations:
            if(dev["id"] == id):
                deviation = dev
                break

        cats = CategorizeSymbol(deviations, membershipFuncs, deviation["value"])
              
        if int(category[0]) in cats:
            basketSymbols.append(column)

    createdBasket = []
    limit = random.randint(4,15);
    if len(basketSymbols)>0:
        rangebasket = len(basketSymbols)
        for b_index in range(7):
            for iterator in range(1,int(limit)):        
                createdBasket.append(basketSymbols[random.randint(0,rangebasket-1)]);
            baskets.append(createdBasket)
            createdBasket = []

    
    return baskets
