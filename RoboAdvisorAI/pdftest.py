



#from PIL import Image
import datetime as d
import pdfkit
import json
import datetime as d
#fpdf.set_global("SYSTEM_TTFONTS", os.path.join(os.path.dirname(__file__),'fonts'))

def create_html():

    time = d.datetime.now().strftime("%d/%m/%Y")

    names = ["SEPET 1", "SEPET 2","SEPET 3","SEPET 4"]
    data = [ [["GARAN","GLYHO","AKBNK"],[ 0.3, 0.5, 0.2]], [["ABDS","ASDDF","FGFG"],[ 0.43, 0.55, 0.2]] ]
    mainHtml = "";
    with open('remake/index.html', 'r', encoding='utf-8') as file:
        mainHtml = file.read()

    sepetlerHtml = ""
    sepetValues = []

    for idx, x in enumerate(data):
        sepetlerHtml += "<div class='container'>"
        sepetlerHtml += "<div>"
        sepetlerHtml += "<div style='display: inline-block; width:33%; text-align:center; margin:auto 0; vertical-align: top;padding: 30px 0 30px 0'><canvas id='chart"+str(idx)+"' width='175' height='175' style='text-align: center;margin:auto 0!important;align-items: center'></canvas></div>"
        sepetlerHtml += "<div style='display: inline-block ;width:55%; text-align:center; margin:auto;padding: 30px 0 30px 0'>"
        sepetlerHtml += "<h5 class='categoryTitle'>"+names[idx]+"</h5>"
        sepetlerHtml += "<table>"
        subArray = []
        for aidx, a in enumerate(x[0]):
            sepetlerHtml += "<tr>"
            sepetlerHtml += "<td class='dotrow'><span class='dot' id='dot"+str(idx)+""+str(aidx)+"'></span></td>"
            sepetlerHtml += "<td>"
            sepetlerHtml += str(x[0][aidx])
            sepetlerHtml += "</td>"
            sepetlerHtml += "<td>% "
            sepetlerHtml += str(x[1][aidx])
            sepetlerHtml += "</td>"
            sepetlerHtml += "</tr>"
            subArray.append(x[1][aidx])
        sepetValues.append(subArray)
        sepetlerHtml += "</table>"
        sepetlerHtml += "</div>"
        sepetlerHtml += "</div>"
        sepetlerHtml += "</div>"
        sepetlerHtml += "</div><p style='padding-left:90px'>Bu varlık sepeti "+time+" tarihinde oluşturulmuştur.</p><center><div> <div style='height: 1px;width: 60%;background: #d0d0d0;margin-top: 80px;margin-bottom: 80px;'></div> </div></center>"
    scriptHtml = "var values = "+json.dumps(sepetValues) +";"

    mainHtml = mainHtml.replace("---SEPETLERHTML---", sepetlerHtml)
    mainHtml = mainHtml.replace("---SCRIPTCHART---", scriptHtml)
    mainHtml = mainHtml.replace("---TIME---", time)

    options = {
        'page-size': 'Letter',
        'encoding': "UTF-8",
        'javascript-delay':'3000',
        'no-outline': None,
        'margin-top': '0.0in',
        'margin-right': '0.0in',
        'margin-bottom': '0.0in',
        'margin-left': '0.0in',
    }
    Html_file= open("remake/filename1.html","w",encoding='utf-8')
    Html_file.write(mainHtml)
    Html_file.close()
    config = pdfkit.configuration(wkhtmltopdf="wkhtmltopdf.exe")
    pdfkit.from_url(['remake/filename1.html'], 'outdump.pdf' ,options=options, configuration=config)
    

create_html()