import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Route, Router } from '@angular/router';

@Component({
  selector: 'app-survey-create',
  templateUrl: './survey-create.component.html',
  styleUrls: ['./survey-create.component.css']
})
export class SurveyCreateComponent implements OnInit {

  model: SurveyModel;
  saving = false;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private router: Router) {
  }

  ngOnInit() {
    this.model = {
      title: '',
      description: '',
      alternatives: [{
        value: ''
      }]
    };
  }

  remove(alternative: { value: string }) {
    this.model.alternatives = this.model.alternatives.filter(x => x !== alternative);
  }

  get valid() {
    return this.model.title &&
      this.model.title &&
      this.model.description &&
      this.model.alternatives.length > 0 &&
      this.model.alternatives.filter(x => !x.value).length === 0;
  }

  add() {
    this.model.alternatives.push({value: ''});
  }

  async save() {
    this.saving = true;
    const id = await this.http.post<string>(this.baseUrl + 'api/survey/', this.model).toPromise();
    this.router.navigate(['/survey/view/', id]);
  }

}

interface SurveyModel {
  title: string;
  description: string;
  alternatives: { value: string }[];
}
