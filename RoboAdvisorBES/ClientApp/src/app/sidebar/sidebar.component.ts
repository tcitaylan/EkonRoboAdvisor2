import { Component, OnInit, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy } from '@angular/core';
import { AuthService } from 'src/_services/auth.service';
import { ContractsService } from 'src/_services/contracts.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class SidebarComponent implements OnInit {
  @Input() userMod: any = {};
  user: any = {};
  username: string;
  reader = new FileReader();
  image: any;
  
  contracts: any;

  constructor(private http: HttpClient, private authService: AuthService,
    public contractservice: ContractsService, public router: Router) { }
  ngOnInit() {
    console.log(this.userMod);
   
    this.contracts = this.contractservice.popContract(5);
    this.reader.onload = (e) => this.image = e.target.result;
    
    // this.changeSelect(this.contracts[0].value + '_' + this.contracts[0].id);
  }

  changeSelect(value: any){
    this.contractservice.changeContract(parseInt(value.split('_')[0], 10));
    this.contractservice.changeContractId(parseInt(value.split('_')[1], 10));
    if (this.router.url === '/tracing') {
      this.restrict('a');
    }
    
  }

  restrict(e) {
    console.log('Sidebor');
    // this.router.navigate['/home'];
    window.location.href = '/home';
    // this.contractservice.redirect();    
  }

  
  logOut() {
    this.authService.logout();
  }
}
