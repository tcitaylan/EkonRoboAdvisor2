import pyodbc 
from datetime import datetime
from datetime import date
from typing import NamedTuple
import statistics
from Settings import *
import time    
from Notifier import *

class DataAccessLayer(object):
    """description of class"""
    conn = pyodbc.connect(connectionString)
    cursor = conn.cursor()

    def __init__(self, ins_var):  
        self.instancevar = ins_var        

    def getRiskCategories():       
        return DataAccessLayer.cursor.execute("select RecordId, CategoryName, MinValue, MaxValue from RiskCategories ORDER BY MinValue")

    def insert_basket_category(basketId, categoryId):
        insertQuery = ("insert into BasketCategory (TempBasketId, RiskCategoryId, RecordDate, CrId) values ('{}', '{}', GETDATE(), 19)".format(basketId, categoryId))
        DataAccessLayer.cursor.execute(insertQuery)
        DataAccessLayer.cursor.commit()

        return True


    def insert_new_advice(Name, Explanation): #creates new basket & returns id

        insertQuery = ("insert into TemplateBaskets (Name, Explanation, RecordDate) values ('{}','{}', GETDATE())".format(Name, Explanation))
        DataAccessLayer.cursor.execute(insertQuery)        
        DataAccessLayer.cursor.commit()

        insertBasket_Category = ("")
        return DataAccessLayer.cursor.execute('select @@IDENTITY').fetchone()[0]

    def insert_basket_symbol(id, sym):

        #for name,value in sym.items():
        symId = str(sym[0]).replace('Asset','')
        if projectType == "hisse":
            insertQuery = ("SELECT COUNT(*) FROM Symbols WHERE Name='"+symId+"'")
            DataAccessLayer.cursor.execute(insertQuery)        
            DataAccessLayer.cursor.commit()

        symWeight = str(sym[1])

        if symId != "Nakit":
            insertQuery = ("insert into TemplateBasketStocks (TemplateBasketID, SymbolID, Perc) values ('{}','{}','{}')".format(id, symId, symWeight))
            DataAccessLayer.cursor.execute(insertQuery)
            DataAccessLayer.cursor.commit()

        return True

    def getSymbols():        

        #query = "select a.* from Symbols a INNER JOIN (select FundId, c = count(0) from SymbolData group by FundId ) b on a.RecordId = b.FundId where b.c > 55";
        query = "select * from Symbols"

        DataAccessLayer.cursor.execute(query);
        return DataAccessLayer.cursor.fetchall()

    def getRequestedSymbols(symarr):        
        cond = " WHERE ("
        for index, sym in enumerate(symarr):
            cond += " RecordId = "+str(sym)
            if index + 1 < len(symarr):
                cond += " OR"
        cond += ")"


        DataAccessLayer.cursor.execute("select * from Symbols"+cond)
        return DataAccessLayer.cursor.fetchall()

    def getHistoricalData(symbolid):
        DataAccessLayer.cursor.execute("select Date,Value from SymbolData Where FundID=" + str(symbolid) + " ORDER BY Date") 
        return DataAccessLayer.cursor.fetchall()

    def getHistoricalDataDate(symbolid,start, end):
        DataAccessLayer.cursor.execute("select Date,Value from SymbolData Where FundID="+str(symbolid)+" AND Date >= '"+start+"' AND Date <= '"+end+"'"+" ORDER BY Date") 
        return DataAccessLayer.cursor.fetchall()

    def getHistoricalDataAll():
        DataAccessLayer.cursor.execute("select FundId,Date,Value from SymbolData ORDER BY Date") 
        return DataAccessLayer.cursor.fetchall()

    def getSymbolsDataByDate(symbols,start, end):
        cond = " WHERE ("
        for index, sym in enumerate(symbols):
            cond += " FundId = "+str(sym)
            if index + 1 < len(symbols):
                cond += " OR"
        cond += ")"

        cond += " AND Date >= '"+start+"' AND Date <= '" + end + "' ORDER BY Date"

        DataAccessLayer.cursor.execute("select FundId,Date,Value from SymbolData " + cond) 
        return DataAccessLayer.cursor.fetchall()

    def symhistoricaldata(sym):
        return DataAccessLayer.cursor.execute('select * from symbolhistoricaldata(\'' + sym + '\')')    #_stored procedure symbolhystoricaldata(sym)

    
    def insert_daily_data(basket, category, name):              
        id = DataAccessLayer.insert_new_advice(name, "Yapay zeka tabanlı sepet.")

        DataAccessLayer.insert_basket_category(id, category[1])
        for item in basket.items(): 
            DataAccessLayer.insert_basket_symbol(id, item)
        return id

    def getSpecialBaskets():
        return DataAccessLayer.cursor.execute('select * from SpecialBaskets') 

    def getSpecialBasketsByCategory(id):
        return DataAccessLayer.cursor.execute('select * from SpecialBaskets where RiskCategoryID='+str(id)) 

    def getSpecialBasketStocks(id):
        return DataAccessLayer.cursor.execute('select * from SpecialBasketStocks where SpecialBasketID='+str(id)) 

    def insertBacktest(backtest, templateBasketId):
        insertQuery = ("insert into TemplateBasketBackTests (TemplateBasketID, Result, RecordDate) values ('{}','{}',GETDATE())".format(templateBasketId, backtest))
        DataAccessLayer.cursor.execute(insertQuery)
        DataAccessLayer.cursor.commit()

    def getTemplateBasket(templateBasketId):
        query = '''select a.Name as BasketName
                    , c.Name
                    , b.Perc  
                    , a.RecordDate
                    from TemplateBaskets a
                    inner join
                    (select TemplateBasketID, SymbolID, Perc from TemplateBasketStocks) b
                    on a.RecordID = b.TemplateBasketID
                    inner join
                    (select Name,Sym,RecordID from Symbols) c
                    on b.SymbolID = c.RecordID
                    where a.RecordID = ''' + str(templateBasketId)
        return DataAccessLayer.cursor.execute(query.replace('\n','')).fetchall()

