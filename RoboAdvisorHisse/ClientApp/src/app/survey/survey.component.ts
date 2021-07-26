import { Component, OnInit, Renderer2 } from '@angular/core';
import { map } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AlertifyService } from 'src/_services/alertify.service';
import { AuthService } from 'src/_services/auth.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-survey',
  templateUrl: './survey.component.html',
  styleUrls: ['./survey.component.css']
})
export class SurveyComponent implements OnInit {

  questions: any = [{}];
  answer: any;
  userAnswers: any = [{}];
  abc = 0;
  choiceAnswer: any;
  constructor(private route: ActivatedRoute, private http: HttpClient, private render: Renderer2, private alertify: AlertifyService,
              private authService: AuthService, private router: Router) { }

  ngOnInit() {
    this.getSurveyModel();

  }

  getSurveyModel() {
    this.http
      .get(environment.apiUrl + '/website/surveyView')
      .subscribe(
        (response) => {
          this.questions = response;
          this.render.addClass(document.getElementById('loading'), 'hide');
          this.render.removeClass(document.getElementById('survey'), 'opc');
        },
        (error) => {
        }
      );
  }

  stepClick(event: any){
    const stepNo = event.target.getAttribute('id').split('-')[1];

    if (this.abc >= stepNo ){
      console.log(event.target + '  ' , stepNo + ' ', this.abc);
      this.render.removeClass(event.target.parentNode.getElementsByClassName('bcsCurrent')[0], 'bcsCurrent');
      this.render.removeClass(event.target, 'bcsSuccess');
      this.render.addClass(event.target, 'bcsCurrent');

      this.render.removeClass(document.querySelector('.surveyContainer.displayB'), 'displayB');
      this.render.removeClass(document.querySelector('.surveyContainer.animated'), 'animated');
      this.render.removeClass(document.querySelector('.surveyContainer.fadeInRight'), 'fadeInRight');
      this.render.removeClass(document.querySelector('.surveyContainer.slideInRight'), 'slideInRight');

      this.render.addClass(document.getElementById('s-' + (stepNo - 1)), 'displayB');
      this.render.addClass(document.getElementById('s-' + (stepNo - 1)), 'animated');
      this.render.addClass(document.getElementById('s-' + (stepNo - 1)), 'fadeInRight');
      this.render.addClass(document.getElementById('s-' + (stepNo - 1)), 'slideInRight');
    }
  }

  choiceSelect(event: any){
    console.log('here');
    // console.log(event.target.getAttribute('data-value'));
  }

  answerFieldClick(event: any){
    if (event.target.getAttribute('data-loc')){
      this.choiceAnswer = event.target.getAttribute('data-loc');
    }
    let id = event.target.id.split('-')[1];
    if (!id){
      id = event.target.parentNode.id.split('-')[1];
    }
    console.log('answer  ' + id);

    this.render.removeClass(document.getElementsByClassName('scaItem')[0], 'scaCurrent' );
    this.render.addClass(event.target, 'scaCurrent');
    this.render.addClass(document.getElementById('rec-' + id), 'displayB');
    this.render.addClass(document.getElementById('rec-' + id), 'animated');
    this.render.addClass(document.getElementById('rec-' + id), 'fadeInRight');

  }

  scButtonClick(event: any){

    const id = event.target.id.split('-')[1];
    const surveryContainer = event.target.parentNode;
    let devamEt = false;
    let soruTuruTextMi = false;
    let soruTuruSayiMi = false;
    let soruTuruSecimMi = false;

    const currentContainerNo = surveryContainer.getAttribute('id').split('-')[1];
    const nextContainerNo =  Number(currentContainerNo) + 1;
    console.log(nextContainerNo);
    console.log(this.abc);
    if (this.abc < Number(currentContainerNo) + 1) {
      this.abc = Number(currentContainerNo) + 1;
      this.render.removeClass(surveryContainer, 'error');
    }

    soruTuruTextMi = surveryContainer.classList.contains('text');
    soruTuruSayiMi = surveryContainer.classList.contains('number');
    soruTuruSecimMi = surveryContainer.classList.contains('choice');

    if (soruTuruTextMi){
      this.answer = surveryContainer.querySelector('input').value;
      const Qid = event.target.id.split('-')[1];
      if (this.answer === '')
      {
        this.render.addClass(surveryContainer, 'error');
      } else {
        this.userAnswers.push({ QuestionID:  Qid, Answer: this.answer });
        devamEt = true;
      }
    } else if (soruTuruSayiMi){
      this.answer = surveryContainer.querySelector('input').value ;
      const Qid = event.target.id.split('-')[1];
      if (this.answer === '' || Number(this.answer) < 1)
      {
        this.render.addClass(surveryContainer, 'error');
      } else {
        this.userAnswers.push({ QuestionID:  Qid, Answer: this.answer });
        devamEt = true;
      }
    } else if (soruTuruSecimMi){
      const Qid = event.target.id.split('-')[1];
      if (this.choiceAnswer === '')
      {
        this.render.addClass(surveryContainer, 'error');
        console.log('err from choice');
      } else {
        console.log(this.choiceAnswer);
        this.userAnswers.push({ QuestionID:  Qid, Answer: this.choiceAnswer });
        devamEt = true;
      }
    } else {
      devamEt = true;
    }

    const answerDto = [];
    if (devamEt === true){
      this.choiceAnswer = '';
      this.render.removeClass(surveryContainer, 'error');

      console.log('nextContainerNo: ' + nextContainerNo);

      if (nextContainerNo > 0 && nextContainerNo <= this.questions.length){
        if (nextContainerNo  === this.questions.length){
           this.alertify.confirm('Anket sonlandırılacak ve cevaplarınız kaydedilecektir. Onaylıyor musunuz?',
           () => {
            let answerdata = {};
            const Dte = new Date();
            let month;
            let day;
            if ((Number(Dte.getMonth() + 1)) < 10)
            {
              month = '0' + (Number(Dte.getMonth() + 1));
            } else { month = (Number(Dte.getMonth() + 1)); }
            if ((Number(Dte.getDate() + 1)) < 10)
            {
              day = '0' + (Number(Dte.getDate() + 1));
            } else { day = (Number(Dte.getDate() + 1)); }
            const Dt = Dte.getFullYear() + '-' + month + '-' + day;
            this.userAnswers.forEach(ans => {
              answerdata = {
                recordDate: Dt,
                recordStatus: true,
                userId: Number(this.authService.decodedToken.nameid),
                questionId: Number(ans.QuestionID),
                answer: ans.Answer,
                surveyId: 0
              };
              console.log(answerDto.push(answerdata));
            });
            answerDto.splice(0, 1);
            this.render.addClass(document.getElementById('survey'), 'opc');
            this.render.removeClass(document.getElementById('loading'), 'hide');
            this.http.post(environment.apiUrl + '/website/postanswers', answerDto).subscribe(
              (res) => {
                console.log('responded');
                window.location = window.top.location;
              }
            );
            });
        } else {
          console.log(document.getElementsByClassName('survey' + (nextContainerNo + 1))[0]);
          this.render.removeClass(surveryContainer, 'slideInRight');
          this.render.addClass(surveryContainer, 'fadeOutLeft');
          this.render.addClass(document.getElementsByClassName('survey' + (nextContainerNo + 1))[0], 'animated');
          this.render.addClass(document.getElementsByClassName('survey' + (nextContainerNo + 1))[0], 'slideInRight');
          this.render.addClass(document.getElementsByClassName('survey' + (nextContainerNo + 1))[0], 'displayB');

          this.render.removeClass(document.getElementsByClassName('bcImage')[0], 'bci0' + nextContainerNo);
          this.render.addClass(document.getElementsByClassName('bcImage')[0], 'bci0' + (nextContainerNo + 1));

          console.log('step-' + nextContainerNo);
          this.render.removeClass(document.getElementById('step-' + nextContainerNo), 'bcsCurrent');
          this.render.addClass(document.getElementById('step-' + (nextContainerNo)), 'bcsSuccess');
          this.render.addClass(document.getElementById('step-' + (nextContainerNo + 1)), 'bcsCurrent');

          this.render.removeClass(surveryContainer, 'animated');
          this.render.removeClass(surveryContainer, 'displayB');
          this.render.removeClass(surveryContainer, 'slideInRight');
        }

      }
    }

  //    }
  //         }
  //     }else if (nextContainerNo == parseInt(@Model.Count)) {
  //                 var dialog = $.confirm({
  //                     title: "Uyarı",
  //                     content: "Anket cevaplarını onaylıyor musunuz?",
  //                     buttons: {
  //                         EVET: function () {
  //                             dialog.close();


  //                             //*********************
  //                             $('#loading').css("display", "block");
  //                             $('.questionPage').css("opacity", "0.3");
  //                             $.each(userAnswers, function(key, value){
  //                                 var answerdata = {
  //                                     "recordDate": "",
  //                                     "recordStatus": "",
  //                                     "userID": parseInt(@Session["UserID"].ToString()),
  //                                     "questionID": value.QuestionID,
  //                                     "answer": value.Answer,
  //                                     "surveyID": 0
  //                                 }
  //                                 answerDto.push(answerdata);
  //                             });

  //                             $.ajax({
  //                                 type: "POST",
  //                                 url: "/Home/PostAnswers",
  //                                 async: false,
  //                                 data: { "answers": answerDto },
  //                                 success: function (response) {

  //                                     if (response.message == "Başarılı") {
  //                                         window.location.href = '/Home/SurveyResult';
  //                                     } else {
  //                                         $(window).ready(hideLoader);
  //                                         setTimeout(hideLoader, 20 * 2000);
  //                                         $('#loading').css("display", "none");
  //                                         $('.questionPage').css("opacity", "1");
  //                                         alert("Bir hata meydana geldi.")
  //                                     }
  //                                 }
  //                             });
  //                         },
  //                         HAYIR: function () { dialog.close(); }
  //                     }
  //                 });
  //             }
  //     else {
  //         $('.error-container').show();

  //     }
  // });
   }

   postAnswers(answers: {})
   {
     let a: {};
     this.http.post(environment.apiUrl + '/website/postanswers', answers).subscribe(
       (res) => {
         a = res;
         return a;
       }
     );
   }
}
