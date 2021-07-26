import smtplib, ssl
from email.mime.text import MIMEText
from email.mime.application import MIMEApplication
from email.mime.multipart import MIMEMultipart
from datetime import datetime
from Settings import *
from os.path import basename

def sendmail(receivers, msg):

    dt_string = datetime.now().strftime("%d/%m/%Y %H:%M:%S")
    port = 465  
    sender_email = "info@ekonteknoloji.com"
    password = "C9dv1e2b"

    mesg = MIMEMultipart()
    #mesg.attach(MIMEText(dt_string + " tarihinde oluşturulan robo yapay zeka sepetleri;\n " + msg))
    mesg.attach(MIMEText("Merhaba,\n\nEkte, " + dt_string + " tarih ve saatinde oluşturulan robo yapay zeka sepetlerini içeren dosya bulunmaktadır. \n\nSaygılarımızla,"))
    mesg['To'] = receivers
    mesg['Bcc'] = bcc
    #for r in receivers:
    #    mesg['To'] = ", ".join(r)
    mesg['From'] = sender_email
    mesg['Subject'] = dt_string + " tarihli robo yapay zeka sepetleri ::" 
    _name_= ""
    with open(".\\outdump.pdf", "rb") as fil:
            _name_ = ".\\AI-" + header + "-Report.pdf"
            part = MIMEApplication(
                fil.read(),                
                Name=basename(_name_)
            )
    # After the file is closed
    part['Content-Disposition'] = 'attachment; filename="%s"' % basename(_name_)
    mesg.attach(part)

    context = ssl.create_default_context()
    recipients = receivers
    try:
        server = smtplib.SMTP_SSL("smtp.zoho.com", port, context=context)
        server.ehlo()
        # server.starttls(context=context)
        server.ehlo()
        log = server.login(sender_email, password)        
        # server.connect()
        recip = receivers.split(', ')
        recip.append(bcc)
        server.sendmail(sender_email, recip, mesg.as_string())
        # server.sendmail(sender_email, "taylancivaoglu@gmail.com", message)
        print("Email sent");
    except Exception as e:
        print(e)
    finally:
        server.quit()

#sendmail(EmailRecipents, "mailitem")



