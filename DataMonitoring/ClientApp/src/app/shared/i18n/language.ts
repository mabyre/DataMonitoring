export class Language {

  public static fromJson(json: Object): Language {
    return new Language(
      json['key'],
      json['alt'],
      json['title'],
      json['culture']
    );
  }

  constructor(public key: string,
    public alt: string,
    public title: string,
    public culture:string) {
  }
}
