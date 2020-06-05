export class User {

    public email: string;
    public emailVerified: false;
    public givenName: string;
    public firstName: string;
    public lastName: string;
    public address: string;
    public society: string;
    public phone: string;

    public gender: string;
    public googleId: string;
    public gravatarId: string;

    public isAdmin: boolean; // SuperAdmin

    public applicationScopes: string[];
    public roles: string[];
    public permissions: string[];

}
