import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { ChartModel } from '../_interfaces/chartmodel.model';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  //public data: ChartModel[] = [];
  public bradcastedData: ChartModel[] = [];

  private hubConnection: signalR.HubConnection | null = null;

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:9002/chart')
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch((err) => console.log('Error while starting connection: ' + err));
  };

  /*
  public broadcastChartData = () => {
    this.hubConnection?.invoke('resetbroadcastchartdata', this.data)
      .catch((err) => console.error(err));
  };
  */

  public startBroadcastChartData = () => {
    this.hubConnection?.invoke('startbroadcastchartdata')
      .catch((err) => console.error(err));
  };

  public endBroadcastChartData = () => {
    this.hubConnection?.invoke('endbroadcastchartdata')
      .catch((err) => console.error(err));
  };


  public addBroadcastChartDataListener = () => {
    this.hubConnection?.on('broadcastchartdata', (data) => {
      console.log(data);
      this.bradcastedData = data;
    });
  };
}
