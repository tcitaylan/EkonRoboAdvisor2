import os
from prediction.Training import *
from prediction.Predicter import *
from Backtest import *
from categorization.Fuzzy import *
from data import *
from allocation.BlackLitterman import *
from data import *


#startDate = "2017-05-02"
#endDate   = "2020-04-26"

#TrainAllSymbols()

#df = Preprocess.PrepareDataframe("*", "2019-05-02", "2020-05-02")
#for column in df.columns:
    
#    plt.figure(figsize=(10,6))
#    plt.plot(df.index.values, df[column].values, color='blue', label='Total Balance')
#    plt.title("")
#    plt.xlabel('Date')
#    plt.ylabel('Total Balance')
#    plt.legend()
#    ax = plt.gca()
#    ax.xaxis.set_major_locator(matplotlib.dates.MonthLocator())
#    ax.xaxis.set_major_formatter(matplotlib.dates.DateFormatter('%m-%Y'))
#    plt.xticks(rotation=45)
#    #plt.show()
#    plt.savefig("saved_backtests\{}.png".format(column))
#    plt.close()

CalculateBacktest("2019-01-01", str(datetime.date.today())) 

#TrainSymbol(41)



#predictions = PredictAllSymbolsByDate("2018-01-03","2018-05-03")
#predictions = OrderByPrediction(predictions)
#print(predictions)


#RandomBacktest(3, 5, startDate, endDate)



#df = Preprocess.PrepareDataframe("*", startDate, endDate)
#devs = Preprocess.StandartDeviation(df)
#membershipFuncs = CreateFuzzyClasses(devs, 1)
#PlotFuzzyChart(devs, membershipFuncs)
#categories = CategorizeSymbol(devs, membershipFuncs, devs[0]["value"])
#categories2 = CategorizeSymbol(devs, membershipFuncs, devs[1]["value"])
#categories3 = CategorizeSymbol(devs, membershipFuncs, devs[2]["value"])
#categories4 = CategorizeSymbol(devs, membershipFuncs, devs[3]["value"])
#categories5 = CategorizeSymbol(devs, membershipFuncs, devs[4]["value"])
#categories6 = CategorizeSymbol(devs, membershipFuncs, devs[5]["value"])
#categories7 = CategorizeSymbol(devs, membershipFuncs, devs[6]["value"])
#print(categories)


#df = Preprocess.PrepareDataframe("*", startDate, endDate)
#weights = Allocate(df, predictions)

#RealBacktest(startDate, endDate, weights, 'Kaleci')
#print(weights)

    



