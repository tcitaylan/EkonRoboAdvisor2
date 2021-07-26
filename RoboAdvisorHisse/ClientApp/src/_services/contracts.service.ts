import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ContractsService {

  exitingContract: number;
  restriction = true;

  contractAmount: number;
  contractId: number;

  contractChange: Subject<number> = new Subject<number>();
  contAmount = this.contractChange.asObservable();

  contractIdChange: Subject<number> = new Subject<number>();
  contId = this.contractIdChange.asObservable();

  userContracts = [{ name: 'Sözleşme 1', value: 10000, id: 1 }, { name: 'Sözleşme 2', value: 7000, id: 2 },
  { name: 'Sözleşme 3', value: 5000, id: 3 }];

  invbasketid: number;
  hascontract = false;

  constructor(public router: Router) { }

  changeContract(contvalue: number) {
    this.contractChange.next(contvalue);
    this.contractAmount = contvalue;
  }

  changeContractId(contId: number) {
    this.contractIdChange.next(contId);
    this.contractId = contId;
  }

  popContract(id: number) {
    this.userContracts.forEach((e, index) => {
      if (id === e.id) {
        const i = this.userContracts.indexOf(e);
        this.userContracts.splice(i, 1);
      }
    });
    return this.userContracts;
  }

}
