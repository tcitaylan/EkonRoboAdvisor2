import { Component, OnInit, HostListener, Renderer2 } from '@angular/core';
import { AuthService } from 'src/_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'ERoboWebSite';
  jwtHelper = new JwtHelperService();
  dtoken: any;

  constructor(public authService: AuthService, public render: Renderer2) {}

  ngOnInit() {
    const token = sessionStorage.getItem('token');

    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
      this.authService.uid = this.jwtHelper.decodeToken(token).nameid;
      console.log('from App: ' + this.authService.uid);
      this.authService.category = true;

    }
  }

  @HostListener('window:onbeforeunload', ['$event'])
  beforeunloadHandler(event){
    localStorage.clear();
    sessionStorage.clear();
  }
}
