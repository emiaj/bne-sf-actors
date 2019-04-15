import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-survey-list',
  templateUrl: './survey-list.component.html',
  styleUrls: ['./survey-list.component.css']
})
export class SurveyListComponent implements OnInit {

  loading = true;
  items: SurveyListItem[] = [];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  async ngOnInit() {
    await this.load();
  }

  private async load() {
    this.loading = true;
    this.items = await this.http.get<SurveyListItem[]>(this.baseUrl + 'api/survey/list').toPromise();
    this.loading = false;
  }

  async remove(id: string) {
    await this.http.delete(this.baseUrl + 'api/survey/' + id);
    await this.load();
  }

}

interface SurveyListItem {
  title: string;
  description: string;
  id: string;
}

