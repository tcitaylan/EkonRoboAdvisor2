import requests
from Settings import *

class DataService(object):
    def __init__(self):  
        self.resp = requests.get(dataServiceUrl+'/Data/Getsymbols')

    i = 0
    # for symbol in resp.json():
    #     if symbol['market'] == 'IMKBH' and not (len(symbol['name']) == 6 and symbol['name'][5] == 'V'):
    #         i+=1
    #         print('{} {}'.format(symbol['name'], symbol['market']))
    #         historicalData = requests.get(dataServiceUrl+'/Data/GetHistoricalData?symbol=' + symbol['name'] + '&period=5&&start=2019-04-01&end=2020-04-28')
    #         for data in historicalData.json():
    #             print('symbol: {} price: {} date: {}'.format(symbol['name'], data['high'], data['date']))
    
    def getRequestedSymbols(symarr):
        symbols = []
        symbolItem = []
        resp = requests.get(dataServiceUrl+'/Data/Getsymbols')
        for symbol in resp.json():
            symbolItem = []
            if symbol['name'] in symarr:
                symbols.append([symbol['name'],symbol['name']])
        return symbols;

    def getHistoricalData(symbolname):
        symbolData = []
        symbolItem = []
        historicalData = requests.get(dataServiceUrl+'/Data/GetCandleData?symbol=' + symbolname + '&period=1440&count=1000')
       
        for data in historicalData.json():
            symbolItem = []
            for item in data.items():
                symbolItem.append(item[1])
            symbolData.append(symbolItem)
        return symbolData
    
    def getHistoricalDataDate(symbolname, start, end):
        symbolsData = []            
        response = (requests.get(dataServiceUrl+'/Data/GetCandleData?symbol=' + symbolname + '&period=1440&count=1000')).json()
        for data in response:
            if(data['date'].split('T')[0] >= start):
                symbolsData.append([symbolname, data['close']])           
        return symbolsData

    def getHistoricalDataAll():
        symbolData = []
        resp = requests.get(dataServiceUrl+'/Data/Getsymbols')
        for symbol in resp.json():
            if symbol['market'] == 'IMKBH' and not (len(symbol['name']) > 5) and not (len(symbol['name']) == 6 and symbol['name'][5] == 'V'):
                i+=1
                print('{} {}'.format(symbol['name'], symbol['market']))
                historicalData = requests.get(dataServiceUrl+'/Data/GetCandleData?symbol=' + symbol['name'] + '&period=1440&count=1000')
                for data in historicalData.json():
                    if(data['date'].split('T')[0] >= start):
                        formatedData = {'name':symbol['name'],'value':data['close'],'date':data['date'].split('T')[0]}
                        symbolData.append(formatedData)
        return symbolData;

    def getSymbolsDataByDate(symarr, idarr, start, end):
        symbolsData = []    
        for index, sym in enumerate(symarr):
            try:
                response = (requests.get(dataServiceUrl+'/Data/GetCandleData?symbol=' + sym + '&period=1440&count=1000')).json()
                for data in response:
                    if(data['date'].split('T')[0] >= start):
                        symbolsData.append([idarr[index], data['date'].split('T')[0], data['close']])
            except:
                print("Data parse error "+sym)
                continue

        return symbolsData
        
    def getRequestedSymbolsData(symarr):        
        symbolsData = []    
        for sym in symarr:
            response = (requests.get(dataServiceUrl+'/Data/GetCandleData?symbol=' + sym +'&period=1440&count=1000')).json()
            for data in response:
                if(data['date'].split('T')[0] >= start):
                    symbolsData.append({'value': data['close'], 'date': data['date'].split('T')[0] })               
        return symbolsData

    #def getLength(self, symbolname):        
    #    r = requests.get(dataServiceUrl+'/Data/GetHistoricalData?symbol=' + symbolname + '&period=1440&start=2019-04-01&end=2020-04-28').json()        
    #    return(len(r))
        




            
# print(DataService().getSymbols())
# print(getHistoricalData('ATLAS'))
# print(DataService().getLength('MEGAP'))
# print(DataService().getRequestedSymbols(['MARKA', 'MEPET', 'OTKAR', 'OSTIM']))
