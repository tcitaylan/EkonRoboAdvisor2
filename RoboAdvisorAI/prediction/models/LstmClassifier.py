import pandas as pd
import os
from sklearn import preprocessing, metrics
from collections import deque
import random
import tensorflow as tf
import time
import numpy
import keras
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense, Dropout, LSTM, BatchNormalization
from tensorflow.keras.callbacks import TensorBoard, ModelCheckpoint

#-----AYARLAR-----
predictionDate = 3
EPOCHS = 5
BATCH_SIZE = 1
NAME = f"LSTM-{int(time.time())}" 
#-----AYARLAR-----
model = Sequential()

def GetModel(inputDimension):
    model = Sequential()
    model.add(LSTM(50, input_shape=inputDimension, return_sequences=True))
    model.add(LSTM(50, input_shape=inputDimension, return_sequences=True))
    model.add(LSTM(50))
    model.add(Dense(1, activation='sigmoid'))

    return model

def CompileModel(model):
    model.compile(loss='mae',
    optimizer=tf.keras.optimizers.Adam(lr=0.001, decay=1e-6),
    metrics=['mae'])

def TrainModel(model, train_x, train_y, val_x, val_y):
    #tensorboard = TensorBoard(log_dir=".\logs\{}".format(NAME))
    filepath = "RNN_Final-{epoch:02d}-{val_acc:.3f}" 
    #checkpoint = ModelCheckpoint("saved_models/{}.model".format(filepath,
    #monitor='val_acc', verbose=1, save_best_only=True, mode='max'))
    #confMatrix = confusionCheckPoint(val_x,val_y)

    model.fit(train_x, train_y,
    batch_size=BATCH_SIZE,
    epochs=EPOCHS,
    validation_data=(val_x, val_y),
    #callbacks=[tensorboard, checkpoint, confMatrix],
    )