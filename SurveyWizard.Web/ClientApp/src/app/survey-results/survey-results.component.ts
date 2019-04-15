import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-survey-results',
  templateUrl: './survey-results.component.html',
  styleUrls: ['./survey-results.component.css']
})
export class SurveyResultsComponent implements OnInit, OnDestroy {

  model: SurveyResultsModel;
  total: number;
  loading = false;
  interval = 0;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private route: ActivatedRoute) {
  }

  async ngOnInit() {

    this.loading = true;

    await this.fetchData();

    this.loading = false;

    this.interval = setInterval(() => {
      this.fetchData();
    }, 10000);

  }

  async fetchData() {
    const id = this.route.snapshot.paramMap.get('id');
    this.model = await this.http.get<SurveyResultsModel>(this.baseUrl + 'api/survey/' + id + '/results').toPromise();
    this.total = Object.entries(this.model.results)
      .map(([, value]) => value)
      .reduce((acc, value) => acc + value, 0);
  }


  ngOnDestroy(): void {
    clearInterval(this.interval);
  }

}

interface SurveyResultsModel {
  id: string;
  description: string;
  title: string;
  results: { [key: string]: number };
}
