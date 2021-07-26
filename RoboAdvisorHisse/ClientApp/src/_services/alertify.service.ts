import { Injectable } from '@angular/core';
import * as alertify from 'alertify.js';

alertify.defaults = {
  modal: true,
  basic: false,
  frameless: false,
  movable: true,
  resizable: true,
  closable: true,
  maximizable: true,
  startMaximized: false,
  pinnable: true,
  pinned: true,
  padding: true,
  overflow: true,
  maintainFocus: true,
  transition: 'pulse',
  notifier: {
      delay: 5,
      position: 'bottom-right'
  }
};

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {


constructor() {
  alertify.defaults = {
    modal: true,
    basic: false,
    frameless: false,
    movable: true,
    resizable: true,
    closable: true,
    maximizable: true,
    startMaximized: false,
    pinnable: true,
    pinned: true,
    padding: true,
    overflow: true,
    maintainFocus: true,
    transition: 'pulse',
    notifier: {
        delay: 5,
        position: 'bottom-right'
    }
  };
 }

  confirm(message: string, okCallback: () => any){
    alertify.confirm(message, (e: any) => {
      if (e) {
        okCallback();
      } else {}
    });
  }

  success(message: string)
  {
    alertify.success(message);
  }

  error(message: string)
  {
    alertify.error(message);
  }

  warning(message: string)
  {
    alertify.warning(message);
  }

  message(message: string)
  {
    alertify.message(message);
  }

}


