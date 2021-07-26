import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

import { AppComponent } from './app.component';
import { OwlModule } from 'ngx-owl-carousel';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { LandingComponent } from './landing-page/landing-page.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { TracingComponent } from './tracing/tracing.component';
import { SurveyresultComponent } from './surveyresult/surveyresult.component';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { AuthService } from 'src/_services/auth.service';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { BotDetectCaptchaModule } from 'angular-captcha';
import { HomeComponent } from './home/home.component';
import { ErrorInterceptor } from 'src/_services/error.interceptor';
import { AlertifyService } from 'src/_services/alertify.service';
import { ChartsModule } from 'ng2-charts';
import { DoughnutChartComponent } from './doughnut-chart/doughnut-chart.component';
import { LineChartComponent } from './line-chart/line-chart.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';

import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { InvestmentComponent } from './investment/investment.component';

import { SurveyComponent } from './survey/survey.component';
import { appRoutes } from './routes';
import localeTr from '@angular/common/locales/tr';
import { registerLocaleData } from '@angular/common';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { ErrorpageComponent } from './errorpage/errorpage.component';
import { ErrorServiceService } from '../_services/error-service.service';

registerLocaleData(localeTr);

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

@NgModule({
  declarations: [
    AppComponent,
    SidebarComponent,
    TracingComponent,
    SurveyresultComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    DoughnutChartComponent,
    LineChartComponent,
    InvestmentComponent,
    SurveyComponent,
    LandingComponent,
    ErrorpageComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    OwlModule,
    CarouselModule,
    BrowserAnimationsModule,
    FormsModule,
    BotDetectCaptchaModule,
    BsDropdownModule.forRoot(),
    ChartsModule,
    PerfectScrollbarModule,
    RouterModule.forRoot(appRoutes, { useHash: true })
  ],
  providers: [
    ErrorInterceptor,
    AuthService,
    AlertifyService,
    ErrorServiceService,
    { provide: LocationStrategy, useClass: HashLocationStrategy }
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
