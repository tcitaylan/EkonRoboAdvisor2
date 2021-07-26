import pandas as pd
import numpy as np
from pypfopt.efficient_frontier import EfficientFrontier
from pypfopt import risk_models
from pypfopt import expected_returns
from pypfopt.discrete_allocation import DiscreteAllocation, get_latest_prices
from pypfopt.black_litterman import *
from datetime import timedelta, date
from Settings import *
from sklearn import preprocessing, metrics
import random
import statistics 

cashStarted = False

#Verilen sembol dataları ve tahminler ile Black Litterman alokasyonu yapar.
def Allocate(symbolDataframe, predictions):
    predarray = {}

    scaledPreds = []

    symbolDataframeCopy = symbolDataframe.copy()

    predictions = sorted(predictions, key=lambda x: x[2], reverse=True)

    cleaned_weights = {}

    means = 0

    try:    
        while len(predictions) > 10:
             lastitem = predictions[len(predictions) - 1]
             del predictions[len(predictions) - 1]
             del symbolDataframeCopy["Asset"+str(lastitem[0])]

        for x in predictions.copy():
            if(np.isnan(x[2])):
                del symbolDataframeCopy["Asset"+x[0]]
                for i,p in enumerate(predictions):
                    if(x[0] == p[0]):
                        del predictions[i]
                        break

        for x in predictions:
            scaledPreds.append(x[2])

        scaledPreds = preprocessing.minmax_scale(scaledPreds, feature_range=(-0.3, 0.9))

        means = statistics.mean(scaledPreds) 

        for index,preds in enumerate(predictions):
            predarray['Asset'+str(preds[0])] = scaledPreds[index]
        
        S = risk_models.sample_cov(symbolDataframeCopy)
            
        bl = BlackLittermanModel(S, absolute_views=predarray)

        returns = bl.bl_returns()

        ef = EfficientFrontier(returns, S, weight_bounds=(allocationBoundaries[0],allocationBoundaries[1])) 

        raw_weights = ef.max_sharpe()

        cleaned_weights = ef.clean_weights()         
        
        ef.portfolio_performance(verbose=True)


        totalSum = 0
        for i in cleaned_weights.copy():
            if cleaned_weights[i] == 0:
                del cleaned_weights[i]

        #sorted(predictions, key=lambda x: x[2], reverse=True)
        #sorted(cleaned_weights.items(), key=lambda x: x[1], reverse=True)

        #if(len(cleaned_weights) < 3):
        #    for index,w in enumerate(predictions):
        #        if("Asset"+str(w[0]) not in cleaned_weights and len(cleaned_weights) < 3):
        #             randInt = random.randint(5,10) / 100
        #             cleaned_weights["Asset"+str(w[0])] = randInt
        #             cleaned_weights[list(cleaned_weights.keys())[0]] -= randInt
    except Exception as e:
        print("EXCEPTION IN BLACK LITTERMANN "+ str(e))

        total = 0

        for x in predictions:
            val = x[2]
            if(x[2] > 0.2):
                val = 0.2
            if(x[2] < 0.05):
                val = 0.05

            total += val
            control = False
            if(total > 1):
                val -= 1 - total
                control = True

            cleaned_weights["Asset"+str(x[0])] = val

            if(control or total >= 1):
                break

        summer = (1-total) / len(cleaned_weights)
        if total < 1:
            for x in cleaned_weights:
                cleaned_weights[x] += summer

    #Tahminler düşükse nakite geçiş
    cleaned_weights["AssetNakit"] = 0

    global cashStarted
    if not cashStarted:
        cashStarted = False
    
    if means > 0.2:
        cashStarted = False

    if means < 0.1 or cashStarted == True:
        cashStarted = True
        for x in cleaned_weights.copy():
            if x == "AssetNakit":
                continue

            if cleaned_weights[x] >= 0.1:
                cashRatio = cleaned_weights[x] / 2
                cleaned_weights[x] -= cashRatio
                cleaned_weights["AssetNakit"] += cashRatio
            else:
                cleaned_weights["AssetNakit"] +=  cleaned_weights[x]
                del cleaned_weights[x]

        if cleaned_weights["AssetNakit"] == 0:
            del cleaned_weights["AssetNakit"]
        else:
            weightsum = 0
            for w in cleaned_weights.copy():
                weightsum += cleaned_weights[w]
            cleaned_weights["AssetNakit"] += (1 - weightsum)

    return cleaned_weights