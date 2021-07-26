
import { AuthService } from 'src/_services/auth.service';
// import { Observable } from 'rxjs';
// import { CaptchaComponent } from 'angular-captcha';
import { RcaptchaserviceService } from 'src/_services/rcaptchaservice.service';
import { Component, OnInit, ViewChild  } from '@angular/core';
import { AlertifyService } from 'src/_services/alertify.service';
import { OwlOptions } from 'ngx-owl-carousel-o';
import { Router } from '@angular/router';



@Component({
  selector: 'app-landing',
  templateUrl: './landing-page.component.html',
    styleUrls: ['./landing-page.component.css']
  // ,providers: [RcaptchaserviceService]
})
export class LandingComponent implements OnInit {
  // @ViewChild(CaptchaComponent, { static: false }) captchaComponent: CaptchaComponent;
  model: any = {};
  registerMode = false;

    customOptions1: OwlOptions = {
        loop: false,
        mouseDrag: true,
        touchDrag: true,
        pullDrag: false,
        dots: true,
        navSpeed: 700,
        nav: true,
        autoHeight: true,
        mergeFit: false,
        items: 1,
        navText: ["<img src='../assets/img/prev.png'>", "<img src='../assets/img/next.png'>"]
    };

  constructor(private authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    if (sessionStorage.getItem('landed')) {
      this.authService.isLanded = true;
    }
  }

  redirect() {
    this.authService.isLanded = true;
    sessionStorage.setItem('landed', 'landed');    
  }

  sendMessage($event) {
      this.alertify.success('Mesajınız alınmıştır. En kısa sürede dönüş yapılacaktır.');
  }

}
