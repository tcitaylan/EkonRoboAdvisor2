import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InvestmentComponent } from './investment/investment.component';
import { HomeComponent } from './home/home.component';
import { SurveyresultComponent } from './surveyresult/surveyresult.component';
import { SurveyComponent } from './survey/survey.component';
import { TracingComponent } from './tracing/tracing.component';


const routes: Routes = [
  // { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'survey/:uid', component: SurveyComponent },
  { path: 'surveyresult/:uid', component: SurveyresultComponent },
  { path: 'investment/:bid', component: InvestmentComponent },
  { path: 'tracing/', component: TracingComponent },
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
