import { cy, Cypress } from 'local-cypress';
import { generatePlexServers, generatePlexLibraries, checkConfig, generateResultDTO, generateSettings, MockConfig } from '@mock';
import {
	DOWNLOAD_API_URL,
	FOLDER_PATH_API_URL,
	NOTIFICATION_API_URL,
	NOTIFICATIONS_HUB_URL,
	PLEX_ACCOUNT_API_URL,
	PLEX_LIBRARY_API_URL,
	PLEX_SERVER_API_URL,
	PLEX_SERVER_CONNECTION_API_URL,
	PROGRESS_HUB_URL,
	SETTINGS_API_URL,
} from '@api-urls';
import { generateDownloadTasks } from '@mock/mock-download-task';

export function basePageSetup(config: Partial<MockConfig> = {}) {
	const configValid = checkConfig(config);

	// PlexServers call
	const plexServers = generatePlexServers(configValid);
	cy.intercept('GET', PLEX_SERVER_API_URL, {
		statusCode: 200,
		body: generateResultDTO(plexServers),
	});

	// PlexServerConnections call
	const plexServerConnections = plexServers.flatMap((x) => x.plexServerConnections);
	cy.intercept('GET', PLEX_SERVER_CONNECTION_API_URL, {
		statusCode: 200,
		body: generateResultDTO(plexServerConnections),
	});

	// PlexLibraries call
	const plexLibraries = plexServers.map((x) => generatePlexLibraries(x.id, configValid)).flat();
	cy.intercept('GET', PLEX_LIBRARY_API_URL, {
		statusCode: 200,
		body: generateResultDTO(plexLibraries),
	});

	const downloadTasks = plexServers.map((x) => generateDownloadTasks(x.id, config)).flat();
	cy.intercept('GET', DOWNLOAD_API_URL, {
		statusCode: 200,
		body: generateResultDTO(downloadTasks),
	});

	cy.intercept('GET', FOLDER_PATH_API_URL, {
		statusCode: 200,
		body: generateResultDTO([]),
	});

	cy.intercept('GET', PLEX_ACCOUNT_API_URL, {
		statusCode: 200,
		body: generateResultDTO([]),
	});

	cy.intercept('GET', SETTINGS_API_URL, {
		statusCode: 200,
		body: generateResultDTO(generateSettings(configValid)),
	});

	cy.intercept('GET', NOTIFICATION_API_URL, {
		statusCode: 200,
		body: generateResultDTO([]),
	});

	cy.intercept('GET', PROGRESS_HUB_URL + '/*', {
		statusCode: 200,
		body: {},
	});

	cy.intercept('GET', NOTIFICATIONS_HUB_URL + '/*', {
		statusCode: 200,
		body: {},
	});
}

export default function route(path: string) {
	return Cypress.env('BASE_URL') + path;
}
