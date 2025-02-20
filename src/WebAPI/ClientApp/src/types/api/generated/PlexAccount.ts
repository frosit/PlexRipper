/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

import type { RequestParams } from "./http-client";
import { ContentType } from "./http-client";

import type { PlexAccountDTO, ResultDTO } from "./data-contracts";

import { apiCheckPipe } from "@api/base";
import Axios from "axios";
import queryString from "query-string";
import { from } from "rxjs";

export class PlexAccount {
  /**
 * No description
 *
 * @tags Plexaccount
 * @name CreatePlexAccountEndpoint
 * @request POST:/api/PlexAccount/

 */
  createPlexAccountEndpoint = (data: PlexAccountDTO, params: RequestParams = {}) =>
    from(
      Axios.request<ResultDTO>({
        url: `/api/PlexAccount/`,
        method: "POST",
        data: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),
    ).pipe(apiCheckPipe<ResultDTO>);

  /**
 * No description
 *
 * @tags Plexaccount
 * @name DeletePlexAccountByIdEndpoint
 * @request DELETE:/api/PlexAccount/{plexAccountId}

 */
  deletePlexAccountByIdEndpoint = (plexAccountId: number, params: RequestParams = {}) =>
    from(
      Axios.request<ResultDTO>({
        url: `/api/PlexAccount/${plexAccountId}`,
        method: "DELETE",
        format: "json",
        ...params,
      }),
    ).pipe(apiCheckPipe<ResultDTO>);

  /**
 * No description
 *
 * @tags Plexaccount
 * @name GetPlexAccountByIdEndpoint
 * @request GET:/api/PlexAccount/{plexAccountId}

 */
  getPlexAccountByIdEndpoint = (plexAccountId: number, params: RequestParams = {}) =>
    from(
      Axios.request<PlexAccountDTO>({
        url: `/api/PlexAccount/${plexAccountId}`,
        method: "GET",
        format: "json",
        ...params,
      }),
    ).pipe(apiCheckPipe<PlexAccountDTO>);

  /**
 * No description
 *
 * @tags Plexaccount
 * @name GeneratePlexTokenEndpoint
 * @request GET:/api/PlexAccount/generate-token/{plexAccountId}

 */
  generatePlexTokenEndpoint = (
    plexAccountId: number,
    query?: {
      /** @default "" */
      verificationCode?: string;
    },
    params: RequestParams = {},
  ) =>
    from(
      Axios.request<String>({
        url: `/api/PlexAccount/generate-token/${plexAccountId}`,
        method: "GET",
        params: query,
        format: "json",
        ...params,
      }),
    ).pipe(apiCheckPipe<String>);

  /**
 * No description
 *
 * @tags Plexaccount
 * @name GetAllPlexAccountsEndpoint
 * @request GET:/api/PlexAccount

 */
  getAllPlexAccountsEndpoint = (
    query?: {
      /** @default false */
      enabledOnly?: boolean;
    },
    params: RequestParams = {},
  ) =>
    from(
      Axios.request<PlexAccountDTO[]>({
        url: `/api/PlexAccount`,
        method: "GET",
        params: query,
        format: "json",
        ...params,
      }),
    ).pipe(apiCheckPipe<PlexAccountDTO[]>);

  /**
 * No description
 *
 * @tags Plexaccount
 * @name UpdatePlexAccountByIdEndpoint
 * @request PUT:/api/PlexAccount

 */
  updatePlexAccountByIdEndpoint = (data: PlexAccountDTO, params: RequestParams = {}) =>
    from(
      Axios.request<PlexAccountDTO>({
        url: `/api/PlexAccount`,
        method: "PUT",
        data: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),
    ).pipe(apiCheckPipe<PlexAccountDTO>);

  /**
 * No description
 *
 * @tags Plexaccount
 * @name IsUsernameAvailableEndpoint
 * @request GET:/api/PlexAccount/check

 */
  isUsernameAvailableEndpoint = (
    query: {
      username: string;
    },
    params: RequestParams = {},
  ) =>
    from(
      Axios.request<Boolean>({
        url: `/api/PlexAccount/check`,
        method: "GET",
        params: query,
        format: "json",
        ...params,
      }),
    ).pipe(apiCheckPipe<Boolean>);

  /**
 * No description
 *
 * @tags Plexaccount
 * @name RefreshPlexAccountAccessEndpoint
 * @request GET:/api/PlexAccount/refresh/{plexAccountId}

 */
  refreshPlexAccountAccessEndpoint = (plexAccountId: number, params: RequestParams = {}) =>
    from(
      Axios.request<ResultDTO>({
        url: `/api/PlexAccount/refresh/${plexAccountId}`,
        method: "GET",
        format: "json",
        ...params,
      }),
    ).pipe(apiCheckPipe<ResultDTO>);

  /**
 * No description
 *
 * @tags Plexaccount
 * @name ValidatePlexAccountEndpoint
 * @request POST:/api/PlexAccount/validate

 */
  validatePlexAccountEndpoint = (data: PlexAccountDTO, params: RequestParams = {}) =>
    from(
      Axios.request<PlexAccountDTO>({
        url: `/api/PlexAccount/validate`,
        method: "POST",
        data: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),
    ).pipe(apiCheckPipe<PlexAccountDTO>);
}

export class PlexAccountPaths {
  static createPlexAccountEndpoint = () => queryString.stringifyUrl({ url: `/api/PlexAccount/` });

  static deletePlexAccountByIdEndpoint = (plexAccountId: number) =>
    queryString.stringifyUrl({ url: `/api/PlexAccount/${plexAccountId}` });

  static getPlexAccountByIdEndpoint = (plexAccountId: number) =>
    queryString.stringifyUrl({ url: `/api/PlexAccount/${plexAccountId}` });

  static generatePlexTokenEndpoint = (
    plexAccountId: number,
    query?: {
      /** @default "" */
      verificationCode?: string;
    },
  ) => queryString.stringifyUrl({ url: `/api/PlexAccount/generate-token/${plexAccountId}`, query });

  static getAllPlexAccountsEndpoint = (query?: {
    /** @default false */
    enabledOnly?: boolean;
  }) => queryString.stringifyUrl({ url: `/api/PlexAccount`, query });

  static updatePlexAccountByIdEndpoint = () => queryString.stringifyUrl({ url: `/api/PlexAccount` });

  static isUsernameAvailableEndpoint = (query: { username: string }) =>
    queryString.stringifyUrl({ url: `/api/PlexAccount/check`, query });

  static refreshPlexAccountAccessEndpoint = (plexAccountId: number) =>
    queryString.stringifyUrl({ url: `/api/PlexAccount/refresh/${plexAccountId}` });

  static validatePlexAccountEndpoint = () => queryString.stringifyUrl({ url: `/api/PlexAccount/validate` });
}
