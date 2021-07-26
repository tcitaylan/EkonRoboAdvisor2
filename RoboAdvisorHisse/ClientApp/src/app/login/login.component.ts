
import { AuthService } from 'src/_services/auth.service';
import { Component, OnInit, Renderer2 } from '@angular/core';
import { AlertifyService } from 'src/_services/alertify.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
  // ,providers: [RcaptchaserviceService]
})
export class LoginComponent implements OnInit {
  model: any = {};
  registerMode = false;

  constructor(private authService: AuthService, private alertify: AlertifyService, private render: Renderer2) { }

  ngOnInit() {
  }

  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }

  // validate(value, valid): void {

  //   const userEnteredCaptchaCode = this.captchaComponent.userEnteredCaptchaCode;

  //   // get the id of a captcha instance that the user tried to solve
  //   const captchaId = this.captchaComponent.captchaId;

  //   const postData = {
  //     userEnteredCaptchaCode,
  //     captchaId
  //   };

  //   // post the captcha data to the backend
  //   this.rcaptchaserviceService.send(postData)
  //     .subscribe(
  //       response => {
  //         if (response.success === false) {
  //           // captcha validation failed; reload image
  //           this.captchaComponent.reloadImage();
  //           // TODO: maybe display an error message, too
  //         } else {
  //           // TODO: captcha validation succeeded; proceed with the workflow
  //         }
  //       });
  // }

  registerToggle() {
    this.registerMode = true;
  }

  login() {

    this.render.removeClass(document.getElementById('loading-main'), 'hide');
    this.render.addClass(document.getElementById('loading-main'), 'show');
    this.render.addClass(document.getElementById('questionPage'), 'opc');

    this.authService.login(this.model).subscribe(next => {
      this.render.removeClass(document.getElementById('loading-main'), 'show');
      this.render.addClass(document.getElementById('loading-main'), 'hide');
      this.render.removeClass(document.getElementById('questionPage'), 'opc');
      this.alertify.success('Giriş Başarılı');
    }, error => {
      this.render.removeClass(document.getElementById('loading-main'), 'show');
      this.render.addClass(document.getElementById('loading-main'), 'hide');
      this.render.removeClass(document.getElementById('questionPage'), 'opc');
      this.alertify.error('Geçersiz kullanıcı');
    });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    sessionStorage.removeItem('token');
    localStorage.removeItem('token');
    console.log('Logged Out');
  }

}
