import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { SurveyComponent } from './survey/survey.component';
import { SurveyresultComponent } from './surveyresult/surveyresult.component';
import { InvestmentComponent } from './investment/investment.component';
import { AuthGuard } from './_guards/auth.guard';
import { TracingComponent } from './tracing/tracing.component';

import { LoginComponent } from './login/login.component';
import { ErrorpageComponent } from './errorpage/errorpage.component';

export const appRoutes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'survey', component: SurveyComponent, canActivate: [AuthGuard] },
  { path: 'surveyresult', component: SurveyresultComponent },
  { path: 'investment', component: InvestmentComponent, canActivate: [AuthGuard] },
  { path: 'tracing', component: TracingComponent, canActivate: [AuthGuard] },  
  { path: 'errorpage', component: ErrorpageComponent },
  // { path: '**', redirectTo: 'home', pathMatch: 'full'}
];
