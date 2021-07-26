import { Component, OnInit, Renderer2 } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as $ from 'jquery';
import * as ch from 'chart.js';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { AuthService } from 'src/_services/auth.service';
import { environment } from '../../environments/environment';
import { ContractsService } from 'src/_services/contracts.service';
import { OwlOptions } from 'ngx-owl-carousel-o';
import { ChartDataSets } from 'chart.js';
import { Label, Color } from 'ng2-charts';

@Component({
  selector: 'app-tracing',
  templateUrl: './tracing.component.html',
  styleUrls: ['./tracing.component.css', '../../assets/js/Chart.js/Chart.css']
})
export class TracingComponent implements OnInit {

  wm:any = {};
  lastbalance: number;
  firstbalance: number;
  rebalanceUsed: number;
  selectedContAmount: number;

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

  cizgirenkler = ['#9ed36a', '#4f5ec7', '#f20000', '#4fc791', '#ff00ff', '#4f90c7', '#F2B46F', '#e3e30b', '#9933ff'];

  /* Line Chart*/
  lineChartData: ChartDataSets[] = [{}];

  lineChartLabels: Label[] = ['1 ', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'];

  lineChartOptions = {
    legend: {
      display: false
    },
    animation: {
      duration: 1000,
    },
    tooltips: {
      callbacks: {
        label: (tooltipItem, data) => {
          const label = data.datasets[tooltipItem.datasetIndex].label + ':' + tooltipItem.yLabel || '';
          return label;
        }
      }
    },
    scales: {
      yAxes: [{
        ticks: {
          fontSize: 10
        }
      }], xAxes: [{ ticks: { fontSize: 10 } }]
    },
    responsive: true
  };

  lineChartColors: Color[] = [
    {
      // borderColor: 'black',
      // backgroundColor: 'rgba(255,255,0,0.28)'
    }
  ];

  lineChartPlugins = [];
  lineChartType = 'line';

/* Eof Line Char */

  asset: any;
  userid: number;
  basketid = -1
  contractId: number;

  constructor(private http: HttpClient, private auth: AuthService
    , private contract: ContractsService, private renderer: Renderer2) { }

  ngOnInit() {
    this.selectedContAmount = this.contract.contractAmount;
    console.log('KONTRAT ID:' + this.contract.exitingContract);
    if (this.contract.exitingContract !== 1000 && this.contract.exitingContract !== undefined) {
      this.contractId = this.contract.exitingContract;
      sessionStorage.setItem('contracted', this.contractId.toString());
    }
    else {
      this.contractId = this.contract.contractId;
    }
    
    this.userid = this.auth.decodedToken.nameid;
    if (this.contract.invbasketid > 0) {
      this.basketid = this.contract.invbasketid;
    }
    
    console.log(this.contractId);
    this.getWebSiteModel();

    $('#left_rebalance').html(3);
        
  }

  getWebSiteModel() {
    this.http.get(environment.apiUrl + '/website/tracing?userid=' + this.userid +
    '&basketid=' + this.basketid + '&contractid=' + this.contractId).subscribe(response => {
      this.wm = response;
      this.renderer.addClass(document.getElementById('loading-main'), 'hide');
      this.renderer.removeClass(document.getElementById('rout'), 'opc');

      /** Date Labels */
      const dateArray: string[] = [];
      this.wm.backtest[0].date.forEach(element => {
        dateArray.push(this.formatDate(element));
      });
      this.lineChartLabels = dateArray.reverse();
    /** eof Date Labels */

      let i = 0;
      this.wm.backtest[0].data.forEach((data, index)  => {
        this.wm.backtest[0].data[index] = data * 100;
      });
      this.wm.backtest.forEach(element => {
        for (let x = 0; x < element.data.length; x++) {
          element.data[x] = element.data[x].toFixed(2);
        }
        if (i === 1) {
          this.lineChartData.push({
            data: element.data.reverse(),
            label: 'Yeni Portföyünüz',
            borderColor: this.cizgirenkler[i],
            borderDash: [10, 2],
            backgroundColor: this.cizgirenkler[i],
            fill: false
          });
          const lab: HTMLDivElement = this.renderer.createElement('span');
          this.renderer.addClass(lab, 'yeni');
          lab.innerHTML = 'Yeni Portföyünüz';
          this.renderer.appendChild(document.getElementById('lineChart'), lab);
          
        } else {
          this.lineChartData.push({
            data: element.data.reverse(),
            label: element.basketname,
            borderColor: this.cizgirenkler[i],
            backgroundColor: this.cizgirenkler[i],
            fill: false
          });
          const lab: HTMLDivElement = this.renderer.createElement('span');
          this.renderer.addClass(lab, 'eski');
          lab.innerHTML = 'Eski Portföyünüz';
          this.renderer.appendChild(document.getElementById('lineChart'), lab);  
        }                     
        i+=1;
      });
            
      this.lastbalance = this.wm.lastBalance * 100;
      this.firstbalance = this.wm.firstBalance;
      this.rebalanceUsed = 3;      
      this.asset = this.wm.assets;
    }, error => {
      console.log(error);
    });
  }

  perc(amount: number) {
    Number(this.firstbalance)
    const a: number = amount * 100;
    const b: number = this.wm.basket.contractAmount;
    const c: number = a / b;
    const d: number = c * 100;
    return d.toFixed(0);
  }


  /** Date Format */
  formatDate(date: string) {
    date = date.split('T')[0];
    const monthNames = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'];
    const d = new Date(date);
    const month = date.split('-')[1];
    const day = date.split('-')[2];
    const year = date.split('-')[0];
    return monthNames[parseInt(month, 10) - 1] + ' ' + year.toString().substring(4 / 2);
  }



}

