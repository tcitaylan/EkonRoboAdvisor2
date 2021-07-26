
######## METLIFE SETTINGS #########

#dataSource = "db" #db, service
#categorizationTypes = "specialbaskets" #linear, fuzzy, specialbaskets
#projectType = "bes" #bes, fon, hisse
#connectionString = 'Driver={SQL Server};Server=192.168.250.10,1433\SQLEXPRESS;Database=EkonRoboDB;Trusted_Connection=no;UID=sa;PWD=Fidelio06;'
#dataServiceUrl = "http://185.122.200.217:6778"
#savedModelPath = "C:\\RoboModels\\Metlife\\models"
#savedBacktestPath = "C:\\RoboModels\\Metlife\\backtests"
#allocationBoundaries = [0.1,1]
#minRebalanceDay = 5
#header = "METLIFE - FON"

######## METLIFE SETTINGS #########



######### TEFAS SETTINGS #########

dataSource = "db" #db, service
categorizationTypes = "linear,specialbaskets" #linear, fuzzy, specialbaskets
projectType = "fon" #bes, fon, hisse
connectionString = 'Driver={SQL Server};Server=192.168.250.10,1433\SQLEXPRESS;Database=EkonRoboDBFon;Trusted_Connection=no;UID=sa;PWD=Fidelio06;'
dataServiceUrl = "http://185.122.200.217:6778"
savedModelPath = "C:\\RoboModels\\Tefas\\models"
savedBacktestPath = "C:\\RoboModels\\Tefas\\backtests"
allocationBoundaries = [0.05,0.2]
minRebalanceDay = 3
header = "FON"

######### TEFAS SETTINGS #########




########## BIST SETTINGS #########

#dataSource = "service" #db, service
#categorizationTypes = "specialbaskets" #linear, fuzzy, specialbaskets  ***AKİF isteği üzerine sadece "special basket" lar***
#projectType = "hisse" #bes, fon, hisse
#connectionString = 'Driver={SQL Server};Server=192.168.250.10,1433\SQLEXPRESS;Database=EkonRoboDBHisse;Trusted_Connection=no;UID=sa;PWD=Fidelio06;'
#dataServiceUrl = "http://185.122.200.217:6778"
#savedModelPath = "C:\\RoboModels\\Bist\\models"
#savedBacktestPath = "C:\\RoboModels\\Bist\\backtests"
#allocationBoundaries = [0.05,0.2]
#minRebalanceDay = 3
#header = "HİSSE"

########## BIST SETTINGS #########

#EmailRecipents = 'mehmetakif.yildiztekin@denizbank.com, ekin@ekonteknoloji.com, abdulaziz.ay@denizbank.com, taylan@ekonteknoloji.com, sadrettin.bagci@denizbank.com.tr'
EmailRecipents ="taylan@ekonteknoloji.com, ekin@ekonteknoloji.com"
#bcc ='ilkeronurkaya@gmail.com,eywinapps@gmail.com'
bcc = ""