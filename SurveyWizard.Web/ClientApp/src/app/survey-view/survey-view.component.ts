import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-survey-view',
  templateUrl: './survey-view.component.html',
  styleUrls: ['./survey-view.component.css']
})
export class SurveyViewComponent implements OnInit {

  model: SurveyModel;
  loading = false;
  mode = 1;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private route: ActivatedRoute) {
  }

  async ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.loading = true;
    this.model = await this.http.get<SurveyModel>(this.baseUrl + 'api/survey/' + id).toPromise();
    this.loading = false;
  }

  async submit() {
    this.mode = 2;
    const id = this.route.snapshot.paramMap.get('id');
    await this.http.post(this.baseUrl + 'api/survey/' + id + '/vote', this.mode);
  }


}

interface SurveyModel {
  id: string;
  title: string;
  description: string;
  alternatives: { value: string }[];
  vote: { value: string };
}
