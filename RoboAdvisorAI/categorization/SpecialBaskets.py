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


def GetSpecialBaskets(df, category):
    specialBaskets = DAL.getSpecialBasketsByCategory(category[1])
    specialBaskets = [tuple(x) for x in specialBaskets]

    baskets = []
    names = []

    for basket in specialBaskets:
        assets = DAL.getSpecialBasketStocks(basket[0])
        assets = [tuple(x) for x in assets]

        basketStocks = []
        for asset in assets:
            if "Asset"+str(asset[2]) in df.columns:
                basketStocks.append("Asset"+str(asset[2]))

        baskets.append(basketStocks)
        names.append(basket[1] +" ("+basket[2]+")")

    return names, baskets
