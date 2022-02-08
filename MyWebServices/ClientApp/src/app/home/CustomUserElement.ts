export class CustomUserElement {
  public Id: number = 0;
  public Name: string = '';
  public Value: string = '';
  public TemplateValue: string | null = null;
  public ElementSotringOrder: SortingOrder | null = null;
  public UserPatternId: number | null = null;
}

enum SortingOrder {
  BeforeText,
  AfterText
}
