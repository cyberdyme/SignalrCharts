import { Component, OnInit } from '@angular/core';
import { SignalRService } from './services/signal-r.service';
import { HttpClient } from '@angular/common/http';

type ChartType =
  | 'line'
  | 'bar'
  | 'horizontalBar'
  | 'radar'
  | 'doughnut'
  | 'polarArea'
  | 'bubble'
  | 'pie'
  | 'scatter';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  public chartOptions: any = {
    scaleShowVerticalLines: true,
    responsive: true,
    scales: {
      yAxes: [
        {
          ticks: {
            beginAtZero: true,
          },
        },
      ],
    },
  };
  public chartLabels: string[] = ['Real time data for the chart'];

  public chartType: ChartType = 'bar';
  public chartLegend: boolean = true;
  public colors: any[] = [
    { backgroundColor: '#5491DA' },
    { backgroundColor: '#E74C3C' },
    { backgroundColor: '#82E0AA' },
    { backgroundColor: '#E5E7E9' },
  ];

  constructor(
    public signalRService: SignalRService,
    private http: HttpClient
  ) {}

  ngOnInit() {
    this.signalRService.startConnection();
    // this.signalRService.addTransferChartDataListener();
    this.signalRService.addBroadcastChartDataListener();
    this.startHttpRequest();
  }

  private startHttpRequest = () => {
    this.http.get('https://localhost:9002/api/chart').subscribe((res) => {
      console.log(res);
    });
  };

  /*
  public chartClicked = (event: any) => {
    console.log(event);
    this.signalRService.broadcastChartData();
  };
  */

  public chartClicked = (event: any) => {
    console.log(event);
  };

  startBroadcastChartData() {
    console.log('startBroadcastChartData clicked');
    this.signalRService.startBroadcastChartData();
  }

  endBroadcastChartData() {
    console.log('endBroadcastChartData clicked');
    this.signalRService.endBroadcastChartData();
  }
}
