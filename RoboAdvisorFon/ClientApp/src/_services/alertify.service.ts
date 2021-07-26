import { Injectable } from '@angular/core';
import alertify from 'alertify.js';

// alertify.defaults = {
//   modal: true,
//   basic: false,
//   frameless: false,
//   movable: true,
//   resizable: true,
//   closable: true,
//   maximizable: true,
//   startMaximized: false,
//   pinnable: true,
//   pinned: true,
//   padding: true,
//   overflow: true,
//   maintainFocus: true,
//   transition: 'pulse',
//   notifier: {
//       delay: 5,
//       position: 'bottom-right'
//   },
//   ok: 'TAMAM',
//   cancel: 'İPTAL'
  // language resources
  // glossary: {
  //     // dialogs default title
  //     title: 'AlertifyJS',
  //     // ok button text
  //     ok: 'OK',
  //     // cancel button text
  //     cancel: 'Cancel'
  // },
  // // theme settings
  // theme: {
  //     // class name attached to prompt dialog input textbox.
  //     input: 'ajs-input',
  //     // class name attached to ok button
  //     ok: 'ajs-ok',
  //     // class name attached to cancel button
  //     cancel: 'ajs-cancel'
  // }
//};

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {



constructor() { }

  confirm(message: string, okCallback: () => any){
    // alertify.defaults = {
    //   modal: true,
    //   basic: false,
    //   frameless: false,
    //   movable: true,
    //   resizable: true,
    //   closable: true,
    //   maximizable: true,
    //   startMaximized: false,
    //   pinnable: true,
    //   pinned: true,
    //   padding: true,
    //   overflow: true,
    //   maintainFocus: true,
    //   transition: 'pulse',
    //   notifier: {
    //       delay: 5,
    //       position: 'bottom-left'
    //   },
    //   ok: 'TAMAM',
    //   cancel: 'İPTAL'
    // };
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


