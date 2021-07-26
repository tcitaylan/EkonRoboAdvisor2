import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorServiceService {

  iserr: boolean = false;
  errChange: Subject<boolean> = new Subject<boolean>();
  err = this.errChange.asObservable();

  constructor() { }

  changeError(haserr: boolean) {
    this.errChange.next(haserr);
    this.iserr = haserr;
  }
}
