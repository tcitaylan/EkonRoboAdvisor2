import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { Location } from '@angular/common';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + '/Website/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  category: any;
  cat: {};
  uid: number;
  isLanded = false;

constructor( private http: HttpClient, private location: Location) { }

  login(model: any) {
    console.log(this.baseUrl);
  return this.http.post(this.baseUrl + 'login', model)
    .pipe(
      map((response: any) => {
        const user = response;
        if (user){
          
          sessionStorage.setItem('token', user.token);
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.uid = this.decodedToken.nameid;
        }
      })
    );
  }

  register(model: any){
    console.log(model);
    return this.http.post(this.baseUrl + 'register', model);
  }

  loggedIn(){
    const token = sessionStorage.getItem('token');
    // this.hasCategory(this.decodedToken.nameid);
    return !this.jwtHelper.isTokenExpired(token);
  }

  landed() {

    return this.isLanded;
  }

  logout() {
    sessionStorage.removeItem('token');
    localStorage.removeItem('token');
    console.log('Logged Out');
  }

  hasCategory(uid: any){
    console.log('hasCat:' + uid);

    this.http.get(environment.apiUrl + '/website/getCategory/' + uid )
    .subscribe(
      (response) => {
        console.log(environment.apiUrl + '/website/getCategory/' + uid );
        this.cat = response;
        console.log('reasp: ' + JSON.stringify(response));
        // this.category = response;
        console.log('from auth reasp: ' + this.category);
        return this.cat;
      }
    );
    // return this.cat;
  }


}
