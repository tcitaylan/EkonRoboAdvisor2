import { Component, OnInit, NgModule } from '@angular/core';
import { AuthService } from 'src/_services/auth.service';
import { HttpClient } from '@angular/common/http';
// import { AppRoutingModule } from '../app-routing.module';
import { Router, ActivatedRoute, Routes } from '@angular/router';
import { environment } from '../../environments/environment';
import { ContractsService } from '../../_services/contracts.service';
import { ErrorServiceService } from '../../_services/error-service.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})

export class HomeComponent implements OnInit {
  title = 'ERoboWebSite';
  name = '';
  category: any;
  errore: boolean;

  model: any = {};
  smi: any = {};
  user: any = {};
  nameSurname: string;
  uid: number;

  wmodel: string;
  umodel: string;

  constructor(public authService: AuthService, private http: HttpClient, private router: Router
    , public contractService: ContractsService, private errorp: ErrorServiceService) { }

  ngOnInit() {
    if (this.authService.loggedIn()){
      this.uid = this.authService.decodedToken.nameid;
      this.category = true;
      // this.errorp.changeError(false);
      this.errorp.errChange.subscribe(
        (val) => {
          console.log('SUBS' + val);
          this.errore = val;
        }
      );
      //this.errore = this.errorp.iserr
      this.getWebSiteModel();

    }
   }

  getWebSiteModel() {
    this.http
      .get(environment.apiUrl + '/website/GetWebSiteModel/' + this.uid)
      .subscribe(
        (response) => {
          this.smi = response;         

          this.smi.contractID.forEach(e => {
            this.contractService.popContract(e);
            this.contractService.exitingContract = parseInt(e, 10)
          });

          const c = this.contractService.popContract(10);
          if (c.length > 0) {
            this.contractService.changeContractId(c[0].id);
            this.contractService.changeContract(c[0].value);
          }

          if (!this.smi.contractID[0] || sessionStorage.getItem('contracted') !== null) {
            this.router.navigate(['/surveyresult']);
          } else {
            this.router.navigate(['/tracing'])
          }          
        },
        (error) => {
          this.smi = false;
          console.log(this.smi + '  ' + error);
        }
      );
  }
}
