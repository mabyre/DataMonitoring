export class Connector {
    public id: number;
    public name: string;
    public timeZone: string;
    public apiConnector: ApiConnector;
    public sqlServerConnector: SqlServerConnector;
    public sqLiteConnector: SqLiteConnector;
    public connectorType: string;
    public connection: string;
}

export class SqLiteConnector {
    public connectionString: string;
    public hostName: string;
    public databaseName: string;
    public useIntegratedSecurity: boolean;
    public userName: string;
    public password: string;
}

export class SqlServerConnector {
    public connectionString: string;
    public hostName: string;
    public databaseName: string;
    public useIntegratedSecurity: boolean;
    public userName: string;
    public password: string;
}

export class ApiConnector {
    public baseUrl: string;
    public acceptHeader: string;
    public autorisationType: string;
    public accessTokenUrl: string;
    public clientId: string;
    public clientSecret: string;
    public grantType: string;
    public httpMethod: string;
}



