import { Component, OnInit, HostListener } from '@angular/core';
import { AuthService } from 'src/_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { ContractsService } from '../_services/contracts.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    title = 'ERoboWebSite';
    jwtHelper = new JwtHelperService();
    dtoken: any;

    constructor(public authService: AuthService, private contService: ContractsService) { }

    ngOnInit() {
        const token = sessionStorage.getItem('token');

        if (token) {
            this.authService.decodedToken = this.jwtHelper.decodeToken(token);
            this.authService.hasCategory(this.jwtHelper.decodeToken(token).nameid);
            this.authService.uid = this.jwtHelper.decodeToken(token).nameid;
            this.authService.category = true;
        }
    }

    @HostListener('window:onbeforeunload', ['$event'])
    beforeunloadHandler(event) {
        localStorage.clear();
        sessionStorage.clear();
    }
}
