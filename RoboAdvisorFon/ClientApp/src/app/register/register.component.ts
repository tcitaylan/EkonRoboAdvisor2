import { Component, OnInit, Output, EventEmitter, Renderer2 } from '@angular/core';
import { AuthService } from 'src/_services/auth.service';
import { AlertifyService } from 'src/_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() registerMode = new EventEmitter();
  model: any = {};

  constructor(private authService: AuthService, private alertify: AlertifyService,
    private render: Renderer2) { }

  ngOnInit() {
  }

  register() {
    this.render.removeClass(document.getElementById('loading-main'), 'hide');
    this.render.addClass(document.getElementById('loading-main'), 'show');
    this.render.addClass(document.getElementById('questionPage'), 'opc');    
    this.authService.register(this.model).subscribe(() => {
      this.render.removeClass(document.getElementById('loading-main'), 'show');
      this.render.addClass(document.getElementById('loading-main'), 'hide');
      this.render.removeClass(document.getElementById('questionPage'), 'opc');
      this.alertify.success('Kayıt oluşturulmuştur.');
      this.authService.login(this.model).subscribe();
    }, error => {
        this.render.removeClass(document.getElementById('loading-main'), 'show');
        this.render.addClass(document.getElementById('loading-main'), 'hide');
        this.render.removeClass(document.getElementById('questionPage'), 'opc');
      console.log(JSON.stringify(error.error, undefined, 2));
      this.alertify.error('Geçersiz kayıt girişimi.');
    });
  }

  cancel(){
    console.log(this.model);
    this.registerMode.emit(false);
  }

}
