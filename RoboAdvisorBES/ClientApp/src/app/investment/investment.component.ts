import { Component, OnInit, Renderer2 } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { OwlOptions } from 'ngx-owl-carousel-o';
import { ContractsService } from 'src/_services/contracts.service';
import { AuthService } from 'src/_services/auth.service';
import { environment } from '../../environments/environment';



@Component({
  selector: 'app-investment',
  templateUrl: './investment.component.html',
  styleUrls: ['./investment.component.css']
})
export class InvestmentComponent implements OnInit {

  constructor(private route: ActivatedRoute, private http: HttpClient, private router: Router,
              public contractService: ContractsService, private render: Renderer2, private auth: AuthService) { }

  im: any = {};
  model: any = {};
  selectedContAmount: number;
  selectedContId: number;
  notification: any;
  rebalance: any;
  agree = { checked: false };
  fivep = false;
  tenp = false;
  fifteenp = false;
  sms = false;
  email = false;
  yes = false;
  no = false;

  savingPercentage: any;

  customOptions1: OwlOptions = {
    loop: false,
    mouseDrag: true,
    touchDrag: true,
    pullDrag: false,
    dots: true,
    navSpeed: 700,
    nav: false,
    autoHeight: true,
    mergeFit: false
  };


  ngOnInit() {
    this.selectedContAmount = this.contractService.contractAmount;
    this.selectedContId = this.contractService.contractId;
    this.contractService.contAmount.subscribe(
      (val) => {
        this.selectedContAmount = val;
      }
    );
    this.contractService.contId.subscribe(
      (val) => {
        this.selectedContId = val;
        console.log('Contract Id from listener investment: ' + this.selectedContId);
      }
    );
    // this.route.paramMap.subscribe(params => {
    this.getInvModel(this.contractService.invbasketid.toString());
    //   console.log(params.get('bid'));
    // });
  }

  getInvModel(basketId: string) {
    this.http
      .get(environment.apiUrl + '/website/Investment?basketId=' + basketId + '&contractId=' + this.selectedContId + '&amount='
      + this.selectedContAmount
      + '&userId=' + this.auth.decodedToken.nameid)
      .subscribe(
        response => {
          this.render.addClass(document.getElementById('loading-main'), 'hide');
          this.render.removeClass(document.getElementById('rout'), 'opc');
          this.im = response;
          this.im.userBasket.userId = parseInt(this.auth.decodedToken.nameid, 10);
        },
        error => {
          console.log(error);
        }
      );
  }

  notificationOptionsMulti(event) {
    const sec = event.target.parentNode.children;
    const array = Array.prototype.slice.call(sec);
    console.log(sec);
    if (event.target.classList.contains('settingsCurrent')) {
      this.render.removeClass(event.target, 'settingsCurrent');
      this.notification.replace(' ' + event.target.id, '');

    }
    else {
      this.render.addClass(event.target, 'settingsCurrent');
      this.notification += ' ' + event.target.id;
    }

    console.log(this.notification);
    // this.render.addClass(event.target, 'settingsCurrent');    
   
  }

  notificationOptions(event){
    const sec = event.target.parentNode.children;
    const array = Array.prototype.slice.call(sec);
    console.log(sec);
    array.forEach(element => {
      if (element.classList.contains('settingsCurrent')){
        this.render.removeClass(element, 'settingsCurrent');
      }
    });
    this.render.addClass(event.target, 'settingsCurrent');
    switch (event.target.parentNode.id)
    {
      case 'percentage':
        this.savingPercentage = event.target.id.replace('yuzde', '');
        break;      
      case 'rebalance':
        this.rebalance = event.target.id;
        break;
    }

    console.log(this.savingPercentage + ' ' + this.notification + ' ' + this.rebalance );
  }

  agreement(event){
    const submit = document.getElementById('save');

    if (event.target.classList.contains('bsActive')){
      this.render.removeClass(event.target, 'bsActive');
      this.render.removeClass(submit, 'bsActive');
      this.render.setAttribute(submit, 'disabled', 'disabled');
    } else {
      this.render.addClass(event.target, 'bsActive');
      this.render.addClass(submit, 'bsActive');
      this.render.removeAttribute(submit, 'disabled', '');
    }
  }

  legalContract(event){

  }

  customerContract(event){

  }

  approval(){
    console.log('approved');
    this.render.removeClass(document.getElementById('loading-main'), 'hide');
    this.render.addClass(document.getElementById('rout'), 'opc');

    const SymbolList: any = this.im.symbolList;
    const UserBasket: any = this.im.userBasket;
    const ContractID = this.im.contractID;
    const Contracts: any = {};
    const BasketID = this.im.basketID;
    const BasketName = this.im.basketName;

    this.model.symbolList = SymbolList;
    this.model.userBasket = UserBasket;
    this.model.contractID = this.selectedContId;
    this.model.contracts = [];
    this.model.basketID = BasketID;
    this.model.basketName = BasketName;
    this.model.autobalance = this.rebalance;
    this.model.notification = this.notification;
    this.model.portfoliNotif = this.savingPercentage;
    console.log(JSON.stringify(this.model, undefined, 2));

    this.http.post(environment.apiUrl + '/website/postInvestment', this.model).subscribe(
      (response) => {
        
        this.model.basketId = response;
        this.contractService.invbasketid = this.model.basketId;
        // this.router.navigate(['/tracing/' + this.model.basketId + '/' + this.selectedContId]);
        this.contractService.popContract(this.selectedContId);
        this.router.navigate(['/tracing']);
      }
    );
  }

}
