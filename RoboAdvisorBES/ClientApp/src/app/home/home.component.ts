import { Component, OnInit, Renderer2, NgModule } from '@angular/core';
import { AuthService } from 'src/_services/auth.service';
import { HttpClient } from '@angular/common/http';
// import { AppRoutingModule } from '../app-routing.module';
import { Router, ActivatedRoute, Routes } from '@angular/router';
import { environment } from '../../environments/environment';
import { ContractsService } from '../../_services/contracts.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})

export class HomeComponent implements OnInit {
  title = 'ERoboWebSite';
  name = '';
  category: any;

  model: any = {};
  smi: any = {};
  user: any = {};
  nameSurname: string;
  uid: number;

  wmodel: string;
  umodel: string;

  constructor(public authService: AuthService, private http: HttpClient, private router: Router, private contractService: ContractsService) { }

  ngOnInit() {
    if (this.authService.loggedIn()){
      this.uid = this.authService.decodedToken.nameid;
      this.category = this.hasCategory();
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
          
          
           
          console.log('redirect home');
          if (this.smi.userResult.userScore !== 0 || !this.smi.userResult.userScore !== null) {
            if (!this.smi.contractID[0] || sessionStorage.getItem('contracted') !== null) {
              
              // this.contractService.changeContract();              
              this.router.navigate(['/surveyresult']);
            } else {
              // sessionStorage.setItem('contracted', this.smi.contractID);
              this.router.navigate(['/tracing'])
            }            
          } else {
            this.router.navigate(['/survey']);
          }
        },
        (error) => {
          this.smi = false;
          console.log(this.smi + '  ' + JSON.stringify(error));
        }
      );
  }

  hasCategory(){
    // return this.authService.category;
    console.log('category');
    this.http.get(environment.apiUrl + '/website/GetCategory/' + this.uid)
      .subscribe( (response) => {
        this.category = response;
        console.log('cat4e' + JSON.stringify(this.category));
        return response;
      });
    if (this.category != null){
      console.log('TRUE');
      return true;
    }else {
      console.log('FALSE');
      return false;
    }
  }


}
