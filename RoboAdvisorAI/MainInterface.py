
from PyQt5.QtWidgets import *
import threading
import sys
from PyQt5.QtCore import QThread, Qt, pyqtSignal, QTimer
import time
import datetime
from matplotlib.backends.backend_qt5agg import FigureCanvas, NavigationToolbar2QT as NavigationToolbar
from matplotlib.figure import Figure
from suds.client import Client
import schedule as s
import time
from MainAlgorithm import *

#class AllThread(QThread):     #progress bar
#    change_value = pyqtSignal(int)
#    change_value.signatures
#    def run(self):
#        cnt = 0
#        while cnt < 100:
#            cnt += 1
#            time.sleep(.3)
#            self.change_value.emit(cnt)     



class MApp(QMainWindow):
    def __init__(self):     
        super(MApp,self).__init__() 
        self.wsdl = "http://localhost:55186/Robologgerservice.svc?WSDL"
        self.client = Client(self.wsdl)
        self.i = -1
        self.j = -1
        self.central_widget = QStackedWidget()
        self.setCentralWidget(self.central_widget)
        self.title  = "AI Advisor Processor"
        self.left   = 300
        self.top    = 300
        self.width  = 600
        self.height = 600
        self.setWindowTitle(self.title)
        self.setGeometry(self.left, self.top, self.width, self.height)
        self.InitUI()
        self.show()
    
    #def setProgressValue(self, val): #progress bar
    #    self.progressbar.setValue(val)

    #def setTabPosition(self): #progress bar
    #    self.thread = AllThread()
    #    self.thread.change_value.connect(self.setProgressValue)
    #    self.thread.start()
    
    def printSym(self,val):      
        self.alert.done(0)
        self.label.setText(str(val))
        self.list.insertItem(0, str(val))
        self.result_basket = val

    def printCatSym(self, val):
        self.catlist.insertItem(0, str(self.categories[self.i])+": "+ str(val))

    def threadHandler(self, val):
        k=0
        print(val)   
        if val and self.i <= len(self.categories) - 1:
            while k < 3:
                self.on_b_stop_threads_click()
                id = DAL.insert_new_advice(self.categories[self.i])
                self.log_list.insertItem(0, self.categories[self.i]+" basket header is created with id-> "+str(id))
            
                for item in self.result_basket.items(): 
                    
                    if(DAL.insert_basket_symbol(id, item)):
                        self.log_list.insertItem(1, str(item[0])+" is added with weight: "+ str(item[1]) + " successfully")                 
            
                result = self.client.service.LogInformation(self.categories[self.i]+"- Id-> "+str(id) +" Sepeti oluşturuldu", "Sepet Oluşturma", 19, "AI Advise", False)
                
                self.log_list.insertItem(1, str(k+1)+".Basket Log Created on DataBase")
                k += 1
            if(self.i !=  len(self.categories) - 1):
                self.on_b_run_algorithm_click()
            else:
                result = self.client.service.LogInformation("Grup Oluşturma İşlemleri tamamlandı :: "+str(datetime.datetime.now()), "Grup Oluşturma Tamamlandı", 19, "AI Advise", False) 
                self.log_list.insertItem(0, "Proceesses Completed Successfully")
                #self.on_b_stop_threads_click()        

    def on_b_run_algorithm_click(self):         
        
        try:
            #a = self.categories[self.i]
            #self.thread = CalculateTodaysResults("2019-05-02", str(datetime.date.today()))            
            #self.thread.sym_monitor.connect(self.printSym)            
            #self.thread.cat_sym_monitor.connect(self.printCatSym)   
            #self.thread.thread_monitor.connect(self.threadHandler)
            self.thread.start()            
            
        except Exception as e: print(e)   
        self.i += 1
        
        self.alert = QMessageBox()
        self.alert.setText('Algorithm started to run')        
        self.alert.exec_() 

    def schedule(self):
        s.every(1).minutes.do(self.on_b_run_algorithm_click)

        while True:
            s.run_pending()

    def tick(self):
        self.j+=1
        timer.sleep(5)
        self.timer1.setText(str(self.j))

    def start_scheduler(self):
        time.sleep(4)
        timer = QTimer()
        timer.timeout.connect(self.tick)
        timer.start(1000)
        #x = threading.Thread(target = self.schedule)
        #x.start()

    def on_b_stop_threads_click(self):
        self.thread.terminate()
        self.thread.quit()
        self.thread.exit()       

    def InitUI(self):        
        vbox = QVBoxLayout()
        self.progressbar = QProgressBar()   
        self.label = QLabel('Cleaned Weights')
        self.list = QListWidget()
        self.log_list = QListWidget()
        self.catlist = QListWidget()
        b_stop_threads= QPushButton('Terminate Threads')        
        b_start_scheduler= QPushButton('Schedule Job') 
        b_start_scheduler.clicked.connect(self.start_scheduler)  
        self.timer1 = QLabel('Timer')

        b_run_algorithm = QPushButton('Run Algorithm')
        b_run_algorithm.clicked.connect(self.on_b_run_algorithm_click)       
        b_stop_threads.clicked.connect(self.on_b_stop_threads_click)        
        
        vbox.addWidget(b_run_algorithm)
        vbox.addWidget(self.timer1)
        vbox.addWidget(self.label)
        vbox.addWidget(self.list)
        vbox.addWidget(self.catlist)
        vbox.addWidget(self.log_list)
        vbox.addWidget(self.progressbar)
        vbox.addWidget(b_stop_threads)
        vbox.addWidget(b_start_scheduler)
       
        a = QWidget()
        a.setLayout(vbox)
        self.setCentralWidget(a)

app = QApplication(sys.argv)
app.setStyle("Windows")
wind = MApp()
sys.exit(app.exec())

