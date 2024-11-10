<template>
	<QPage>
		<QSection>
			<template #header>
				{{ t('pages.debug.dialogs.header') }}
			</template>
			<q-markup-table>
				<q-tr>
					<q-td>
						<DebugButton
							:label="t('pages.debug.dialogs.buttons.server-dialog')"
							@click="dialogStore.openServerSettingsDialog(1)" />
					</q-td>
				</q-tr>
				<q-tr>
					<q-td>
						<DebugButton
							:label="t('pages.debug.dialogs.buttons.download-confirmation')"
							@click="openDownloadConfirmationDialog" />
					</q-td>
				</q-tr>
				<q-tr>
					<q-td>
						<DebugButton
							:label="t('pages.debug.dialogs.buttons.help-dialog')"
							@click="openHelpDialog" />
					</q-td>
				</q-tr>
				<q-tr>
					<q-td>
						<DebugButton
							:label="$t('general.commands.add-alert')"
							@click="addAlert" />
					</q-td>
				</q-tr>
				<q-tr>
					<q-td>
						<DebugButton
							data-cy="check-server-connections-dialog-button"
							:label="t('pages.debug.dialogs.buttons.check-server-connections-dialog')"
							@click="openCheckServerConnectionsDialog" />
					</q-td>
				</q-tr>
				<q-tr>
					<q-td>
						<DebugButton
							data-cy="open-verification-dialog-button"
							:label="t('pages.debug.dialogs.buttons.verification-dialog')"
							@click="openVerificationDialog" />
					</q-td>
				</q-tr>
				<q-tr>
					<q-td>
						<DebugButton
							data-cy="directory-browser-dialog-button"
							:label="t('pages.debug.dialogs.buttons.directory-browser')"
							@click="openDirectoryBrowserDialog" />
					</q-td>
				</q-tr>
			</q-markup-table>
			<ServerDialog />
			<DownloadConfirmation />
			<AccountVerificationCodeDialog :account="account" />
			<DirectoryBrowser />
		</QSection>
	</QPage>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { type DownloadMediaDTO, type PlexAccountDTO, PlexMediaType } from '@dto';
import { generateDefaultFolderPaths, generatePlexAccount } from '@factories';
import { DialogType } from '@enums';
import { useAlertStore, useHelpStore, useDialogStore } from '#imports';

const { t } = useI18n();
const helpStore = useHelpStore();
const alertStore = useAlertStore();
const dialogStore = useDialogStore();

const account = ref<PlexAccountDTO>(
	generatePlexAccount({
		id: 1,
		plexLibraries: [],
		plexServers: [],
	}),
);

const folderPath = generateDefaultFolderPaths({})[0];

function openDownloadConfirmationDialog(): void {
	const demo: DownloadMediaDTO[] = [
		{
			plexServerId: 1,
			plexLibraryId: 9,
			mediaIds: [24, 25, 26, 27, 28],
			type: PlexMediaType.TvShow,
		},
	];
	dialogStore.openMediaConfirmationDownloadDialog(demo);
}

function openHelpDialog(): void {
	helpStore.openHelpDialog({
		label: t('help.settings.ui.language.language-selection.label'),
		title: t('help.settings.ui.language.language-selection.title'),
		text: t('help.settings.ui.language.language-selection.text'),
	});
}

function openCheckServerConnectionsDialog(): void {
	dialogStore.openDialog(DialogType.CheckServerConnectionDialogName);
}

function openVerificationDialog(): void {
	dialogStore.openDialog(DialogType.AccountVerificationCodeDialog);
}

function openDirectoryBrowserDialog(): void {
	dialogStore.openDirectoryBrowserDialog(folderPath);
}

function addAlert(): void {
	alertStore.showAlert({ id: 0, title: 'Alert Title 1', text: 'random alert' });
	alertStore.showAlert({ id: 0, title: 'Alert Title 2', text: 'random alert' });
	alertStore.showAlert({ id: 0, title: 'Alert Title 3', text: 'random alert' });
	alertStore.showAlert({ id: 0, title: 'Alert Title 4', text: 'random alert' });
	alertStore.showAlert({ id: 0, title: 'Alert Title 5', text: 'random alert' });
}
</script>
