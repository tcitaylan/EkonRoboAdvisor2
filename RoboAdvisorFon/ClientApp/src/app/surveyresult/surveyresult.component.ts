import { Component, OnInit, Input, Renderer2, ViewEncapsulation } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { registerLocaleData } from '@angular/common';

import { ChartType } from 'chart.js';
import { MultiDataSet, Label } from 'ng2-charts';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { Color } from 'ng2-charts';
import { Observable } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ContractsService } from 'src/_services/contracts.service';
import tr from '@angular/common/locales/tr';
import { environment } from '../../environments/environment';
import { AuthService } from 'src/_services/auth.service';


@Component({
  selector: 'app-surveyresult',
  templateUrl: './surveyresult.component.html',
  styleUrls: ['./surveyresult.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class SurveyresultComponent implements OnInit {
  @Input() uid: number;
  @Input() webModel: {} = {};
  selectedContAmount: number;
  selectedContId: number;
  htmlToAdd = '';
  returnedData$: Observable<any>;
  doughnutChartData: MultiDataSet = [[]];
  dataArray: MultiDataSet[] = [];
  numberArray: number[][] = [];
  labelArray: string[][] = [];
  doughnutChartType: ChartType = 'doughnut';
  donutObject = {};

  first: Observable<string>;
  second: Observable<string>;
  third: Observable<string>;
  fourth: Observable<string>;

  datadate: any[] = [];

  color: string;
  cizgirenkler = ['#fda31c', '#0f75e6', '#f20000', '#4fc791', '#ff00ff', '#4f90c7', '#F2B46F', '#4fbec7', '#e3e30b', '#9933ff'];
  donutrenkler = [[
    {
      backgroundColor:
        // TURUNCU DONUT
        ['#fda31c', '#d17e02', '#c98216', '#cf8c27', '#b37415', '#d18717', '#c47a0a', '#ed9d24', '#ffa824', '#e8a33a']
    }],
  [{
    backgroundColor:
      // MAVİ DONUT
      ['#0f75e6', '#4a8cd5', '#74aff2', '#5893d5', '#4a8cd5', '#4893e6', '#3e99ff', '#287bd8', '#1e82f2', '#0f75e6']
  }],
  [{
    backgroundColor:
      // KIRMIZI DONUT
      ['#f20000', '#e64040', '#c70000', '#ff4545', '#d44646', '#e33e19', '#de2900', '#fa1b44', '#de0b32', '#c90025']
  }],
  [{
    backgroundColor:
      // YESİL DONUT
      ['#4fc791', '#2e9969', '#22bd77', '#14ba6f', '#0aab62', '#039955', '#06bf6b', '#07db7b', '#0af087', '#47c98e']
  }],
  [{
    backgroundColor:
      // FUSYA DONUT
      ['#ff00ff', '#e310e3', '#c430c9', '#bf02bf', '#e01be0', '#b216b8', '#ca08d1', '#e61fed', '#b604bd', '#f842ff']
  }],
  [{
    backgroundColor:
      // KAPALI MAVİ DONUT
      ['#4f90c7', '#2580cf', '#3d83bf', '#2580cf', '#1572c2', '#278ce3', '#0b63b0', '#0066bf', '#007be6', '#038aff']
  }],
  [{
    backgroundColor:
      // ACIK SARI DONUT
      ['#F2B46F', '#ee9f44', '#efab5d', '#cb7719', '#8b4d07', '#ffd9ae', '#ae8c65', '#1a8cff', '#66b3ff', '#0099ff']
  }],
  [{
    backgroundColor:
      // SU MAVİSİ DONUT
      ['#4fbec7', '#0cbfcf', '#2f98a1', '#1c949e', '#23acb8', '#29bfcc', '#11adba', '#15d5e6', '#33e3f2', '#00c8d9']
  }],
  [{
    backgroundColor:
      // SARI DONUT
      ['#e3e30b', '#e0e010', '#e0cb2d', '#e6cb02', '#ffea4f', '#f7dd1b', '#d1bb13', '#e6e600', '#bfbf17', '#ffff33']
  }],
  [{
    backgroundColor:
      // MOR DONUT
      ['#9933ff', '#8c1aff', '#b366ff', '#974be3', '#7428bf', '#b166fa', '#813ac7', '#621ba8', '#5900b0', '#a511cf']
  }],
  ];

  /* Line Chart*/
  lineChartData: ChartDataSets[] = [{}];

  lineChartLabels: Label[] = ['1 ', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'];

  lineChartOptions = {
    legend: {
      display: false
    },
    //series: {
    //  0: { lineDashStyle: [4, 1] }
    //},
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

  lineChartColors: Color[] = [{}];

  lineChartPlugins = [];
  lineChartType = 'line';

  /* Eof Line Char */

  sm: any = {};
  donuChartOptions: any;
  customOptions: {
    loop: false;
    mouseDrag: true;
    touchDrag: true;
    pullDrag: false;
    dots: false;
    navSpeed: 700;
    navText: ['Previous', 'Next'];
    nav: true;
  };

  constructor(private http: HttpClient, private route: ActivatedRoute
    , public auth: AuthService
    , public contractService: ContractsService, private render: Renderer2) { }

  ngOnInit() {
    
        registerLocaleData(tr);
        this.getWebSiteModel(this.auth.uid.toString());
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
            }
        );
    }
    
    getWebSiteModel(uidf: string) {
      
        this.http
            .get(environment.apiUrl + '/website/GetWebSiteModel/' + uidf)
            .subscribe(
                response => {
                this.sm = response;
                console.log("SM::");
                console.log(this.sm);
                    this.render.removeClass(document.getElementById('rout'), 'opc');
                    this.render.addClass(document.getElementById('loading-main'), 'hide');
                    this.render.removeClass(document.getElementById('popupoverlay'), 'hide');
                    this.render.removeClass(document.getElementById('tutorial'), 'hide');

          this.first = this.sm.existingBasket.symbolName[0];
          this.second = this.sm.existingBasket.symbolName[1];
          this.third = this.sm.existingBasket.symbolName[2];
          this.fourth = this.sm.existingBasket.symbolName[3];

                    /** Linechart  */
                    /** Date Labels */
                    const dateArray: string[] = [];
                    //console.log("BT1:");
                    //console.log(this.sm.backTest[1]);
                    //console.log("BT2:");
                    //console.log(this.sm.backTest[2]);
                        this.sm.backTest[1].date.forEach(element => {
                          dateArray.push(this.formatDate(element));                      
                        });
                    this.lineChartLabels = dateArray.reverse();
                    console.log("LINE CHART LABELS");
                    console.log(this.lineChartLabels);
                    /** eof Date Labels */
                    let i = 0;
                    this.sm.backTest.forEach(element => {
                        for (let x = 0; x < element.data.length; x++) {
                            element.data[x] = element.data[x].toFixed(2);
                      }
                      
            if (i === 0) {
              this.lineChartData.push({
                data: element.data.reverse(),
                label: element.basketname,
                borderColor: this.cizgirenkler[i],
                //borderDash: [10, 2],
                backgroundColor: this.cizgirenkler[i],
                fill: false
              });
            } else {
              this.lineChartData.push({
                data: element.data.reverse(),
                label: element.basketname,
                borderColor: this.cizgirenkler[i],
                backgroundColor: this.cizgirenkler[i],
                fill: false
              });
            }

            const lab: HTMLDivElement = this.render.createElement('span');
            this.render.addClass(lab, 'c' + i);
            this.render.addClass(lab, 'hand');
            lab.innerHTML = element.basketname;
            this.render.appendChild(document.getElementById('lineChart'), lab);
            this.render.listen(lab, 'click', ($event) => {
              let included: boolean;
              this.lineChartData.some((e, index) => {
                if (e.label === $event.target.innerHTML) {
                  this.lineChartData.splice(index, 1);
                  included = true;
                  return;
                }
              });
              if (!included) {
                const color = $event.target.classList[0].replace('c', '');
                this.sm.backTest.forEach(ele => {
                  if (ele.basketname === $event.target.innerHTML) {
                    this.lineChartData.push({
                      data: ele.data,
                      label: ele.basketname,
                      borderColor: this.cizgirenkler[color],
                      backgroundColor: this.cizgirenkler[color],
                      fill: false
                    });
                  }
                });
              }
            });
            i++;
          });
          /** eof Linechart  */

          /** Donut Chart */
          for (let k = 0; k < this.sm.templateBaskets.length; k++) {
            let d: MultiDataSet = [[]];
            let ls: string[] = [];

            d = [this.basketSymbols(this.sm.templateBaskets[k].recordId)];
            this.numberArray[k] = this.basketSymbols(this.sm.templateBaskets[k].recordId);
            ls = this.basketSymbolNames(this.sm.templateBaskets[k].recordId);
            for (var ind = 0; ind < ls.length; ind++) {
              if (ls[ind].length > 10) {
                ls[ind] = ls[ind].substring(0, 10) + "...";
              }
            }
            this.dataArray[k] = d;
            this.labelArray[k] = ls;
          }

          this.donutObject = { labels: this.labelArray, datasets: this.numberArray };

          this.donuChartOptions = {
            tooltips: {
              callbacks: {
                label: (tooltipItem, data) => {
                  var inds = 0;

                  for (var iter = 0; iter < this.dataArray.length; iter++) {
                    var control = true;

                    for (var b = 0; b < data.datasets[tooltipItem.datasetIndex].data.length; b++) {
                      if (this.dataArray[iter][0][b] != data.datasets[tooltipItem.datasetIndex].data[b]) {
                        control = false
                      }
                    }

                    if (control == true) {
                      inds = iter;
                    }
                  }

                  const label = this.labelArray[inds][tooltipItem.index] + ' : ' + data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                  return label;
                }
              }
            }
          };

        },
        error => {
          console.log(error);
        }
      );
  }

  onDevam(event) {
    this.render.addClass(document.getElementById('slide1'), 'hide');
    this.render.removeClass(document.getElementById('slide2'), 'hide');

  }

  onClose(event) {
    this.render.addClass(document.getElementById('tutorial'), 'hide');
    this.render.addClass(document.getElementById('popupoverlay'), 'hide');
  }

  invest(basket: any) {
    this.contractService.invbasketid = basket;
    this.render.removeClass(document.getElementById('loading-main'), 'hide');
    this.render.addClass(document.getElementById('rout'), 'opc');
    console.log('Invested ' + JSON.stringify(basket));
  }

  changeSelect(event: any) {
    this.render.removeClass(document.querySelector('.dpcItem.dpcCurrent'), 'dpcCurrent');
    this.render.addClass(event.target.parentNode, 'dpcCurrent');
  }

  symbolName(sId: number) {
    if (sId === 98000) {
      this.returnedData$ = "NAKİT" as any;
    } else {
      this.sm.basketSymbols.forEach(element => {
        if (element.recordId === sId) {
          this.returnedData$ = element.name;
        }
      });
    }
  }

  symbolName2(sId: number) {
    let name = '';
    if (sId === 98000) {
      name = 'NAKİT';
    } else {
      this.sm.basketSymbols.forEach(element => {
        if (element.recordId === sId) {
          name = element.name;
        }
      });
    }
    return name;
  }

  basketSymbols(basketId: number) {
    const templateBasket: number[] = [];
    this.sm.templateBasketStock.forEach(element => {
      if (element.templateBasketId === basketId) {
        templateBasket.push(element.perc.toFixed(1));
      }
    });
    return templateBasket;
  }

  basketSymbolNames(basketId: number) {
    const BasketSymbolNames: string[] = [];
    this.sm.templateBasketStock.forEach(element => {
      if (element.templateBasketId === basketId) {
        BasketSymbolNames.push(this.symbolName2(element.symbolId));
      }
    });
    return BasketSymbolNames;
  }

    /** Date Format */
    formatDate(date: string) {
        date = date.split('T')[0];
        const monthNames = ['Oca', 'Sub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Agu', 'Eyl', 'Eki', 'Kas', 'Ara'];
        const d = new Date(date);
        const month = date.split('-')[1];
        const day = date.split('-')[2];
        const year = date.split('-')[0];
      console.log("DATE: " + monthNames[parseInt(month, 10) - 1] + ' ' + year.toString().substring(4 / 2));
        return monthNames[parseInt(month, 10) - 1] + ' ' + year.toString().substring(4 / 2);
    }
}
