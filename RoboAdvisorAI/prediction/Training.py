
import os
from collections import deque
import numpy
import matplotlib.pylab as plt
from data.DataAccessLayer import DataAccessLayer as DAL
import data.Preprocess as Preprocess
import pandas as pd
import matplotlib.pylab as plt
from sklearn.preprocessing import MinMaxScaler
from prediction.models.LstmClassifier import *
#from prediction.models.SimpleNnClassifier import *
from Settings import *
from data.DataService import DataService as DS

#-----AYARLAR-----
testingSize = 0.20
#-----AYARLAR-----

#Tüm sembolleri eğitir ve kaydeder
def TrainAllSymbols():
    symbols = None

    symbols = DAL.getSymbols()

    AllPredictions = []

    #Sembolleri veritabanından çek ve her biri için uygula: 
    for symbol in symbols:
        try:
            res = TrainSymbol(symbol)
            AllPredictions.append([symbol[0],symbol[1],res])
        except:
            continue

    return AllPredictions
      
#Tek bir sembolü eğitir ve kaydeder
def TrainSymbol(symbol):
    #Sembole ait tarihsel verileri çek ve eğitim verisini oluştur (DataFrame formatında)
    df = Preprocess.CreateDataframe(symbol)

    df = df[:-1]
    #--------Test ve train kümelerini hazırla---------------
    times = sorted(df.index.values)

    testing_df = df.iloc[-int(testingSize*len(times)):]
    training_df = df.iloc[:int(len(times)*(1-testingSize))]

    train_x, train_y = Preprocess.NormalizeDataFrame(training_df,predictionDate)
    test_x, test_y = Preprocess.NormalizeDataFrame(testing_df,predictionDate)
    #--------Test ve train kümelerini hazırla---------------


    #--------Model oluşturma ve eğitim----------------------
    model = GetModel(train_x.shape[1:])
    CompileModel(model)
    history = TrainModel(model,train_x, train_y, test_x, test_y)
    #--------Model oluşturma ve eğitim----------------------


    #Oluşturulan model ile test verisini tahmin et
    prediction = model.predict(test_x)


    #Modeli kaydet
    NAME = str(symbol[0])
    model.save(savedModelPath+"\{}.h5".format(NAME)) 


    #--------Grafik oluştur-------------------------------
    plt.figure(figsize=(10,6))
    plt.plot(test_y, color='red', label='Price')
    plt.plot(prediction, color='blue', label='Prediction')
    plt.title('Prediction')
    plt.xlabel('Date')
    plt.ylabel('Price')
    plt.legend()
    plt.savefig(savedModelPath+"\{}.png".format(NAME))

    return prediction