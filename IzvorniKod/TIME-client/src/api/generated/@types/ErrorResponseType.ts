//detail
// :
// "Validation failed: \n -- NewPassword: The length of 'New Password' must be at least 8 characters. You entered 1 characters. Severity: Error"
// errors
// :
// [{statusCode: 400,…}]
// exceptionDetails
// :
// [{,…}]
// status
// :
// 400
// title
// :
// "FluentValidation.ValidationException"
// traceId
// :
// "00-06963a41b630dfe7a5b99a04f7c801c0-963a76c2a1de103c-00"

export type ErrorResponseType = {
  detail: string;
  status: number;
  title: string;
  traceId: string;
};
