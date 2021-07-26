import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
// tslint:disable-next-line: import-blacklist
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RcaptchaserviceService {

constructor(private http: HttpClient) { }

send(data: object): Observable<any> {
  const options = {
    headers: new HttpHeaders({'Content-Type': 'application/json; charset=utf-8'})
  };

  return this.http.post(
    'http://localhost:5009/simple-captcha-endpoint.ashx', data, options);

  }
}
