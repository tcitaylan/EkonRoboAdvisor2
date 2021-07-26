import os
from collections import deque
import numpy
from prediction.models.LstmClassifier import *
#from prediction.models.SimpleNnClassifier import *
import matplotlib.pylab as plt
from data.DataAccessLayer import DataAccessLayer as DAL
import data.Preprocess as Preprocess
import pandas as pd
import matplotlib.pylab as plt
import os.path
from data.DataService import DataService as DS
from Settings import *
import statistics
from keras import backend as K

#Tahminleme yapılmadan önce model yüklenmelidir.
def LoadModel(symbolid):
    NAME = str(symbolid)
    FILENAME = savedModelPath+"\{}.h5".format(NAME)
    
    exists = os.path.isfile(FILENAME) 

    if exists :
        model = tf.keras.models.load_model(FILENAME, compile=False)
        CompileModel(model)
        return model
    else:
        return "false"

#Tüm semboller için tahminleme yapar
def PredictAllSymbols():

    symbols = DAL.getSymbols()

    AllPredictions = []

    for symbol in symbols:
        K.clear_session()
        model = LoadModel(symbol[0])
        if model != "false" :
            prediction = PredictSymbol(symbol[0],symbol[1],model)
            AllPredictions.append([symbol[0],symbol[1],prediction])

    return AllPredictions

#Tüm sembollerde belirli tarih için tahminleme yapar
def PredictAllSymbolsByDate(start,end):

    symbols = DAL.getSymbols()

    AllPredictions = []

    for symbol in symbols:
        K.clear_session()
        model = LoadModel(symbol[0])
        if model != "false" or len(model) == 0:
            prediction = PredictSymbol(symbol,model,start,end)
            AllPredictions.append([symbol[0],symbol[1],prediction])

    return AllPredictions

#Tek bir sembol için tahminleme yapar 
def PredictSymbol(symbol, model, start = "", end = ""):
    #Sembole ait tarihsel verileri çek ve eğitim verisini oluştur (DataFrame formatında)
    df = pd.DataFrame() 

    if start != "":
        df = Preprocess.CreateDataframeByDate(symbol, start, end)
    else:
        df = Preprocess.CreateDataframe(symbol)

    #Veri önişleme
    test_x, test_y = Preprocess.NormalizeDataFrame(df,predictionDate)

    #Model yüklenmediyse yükle
    if model == None:
        LoadModel(symbol[0])

    #Tahminle
    prediction = model.predict(test_x)
    
    prediction = [x[0] for x in prediction]

    return prediction

#Tahminlenen son veriyi bir önceki tahminden farkına göre sıralar
def OrderByPrediction(predictions):
    orderedList = []
    for x in predictions:
        lastPred = x[2][len(x) - 1]
        lastPred2 = x[2][len(x) - 2]
        profit = lastPred2 - lastPred
        orderedList.append([x[0],x[1],profit])

    orderedList.sort(key=lambda x: x[2], reverse=True)

    return orderedList

#İki tarih arasındaki veriyi kullanarak tahminleme yapar
def PredictSymbolsByDate(symbols, start, end):

    symbols = DAL.getRequestedSymbols(symbols)

    AllPredictions = []

    for symbol in symbols:
        try:
            K.clear_session()
            model = LoadModel(symbol[0])
            if model != "false" :
                prediction = PredictSymbol(symbol,model,start,end)
                AllPredictions.append([symbol[0],symbol[1],prediction])
        except:
            continue

    for pred in AllPredictions:
        newpreds = []
        for index, p in enumerate(pred[2]):
            if index > 6:
                m1 = statistics.mean(pred[2][index-6:index-3]) 
                m2 = statistics.mean(pred[2][index-3:index]) 
                newpreds.append(((m2 - m1) / m2))

        pred[2] = newpreds

    return AllPredictions
