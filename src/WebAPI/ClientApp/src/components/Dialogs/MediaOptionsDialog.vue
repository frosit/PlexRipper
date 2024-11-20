<template>
	<QCardDialog
		width="600px"
		:name="DialogType.MediaOptionsDialog"
		:loading="false"
		close-button
		@closed="$emit('closed')">
		<template #title>
			{{
				$t('components.media-options-dialog.title')
			}}
		</template>
		<!--	Help text	-->
		<template #default>
			<HelpGroup>
				<HelpRow
					center-slot
					:title="$t('help.media-options.hide-offline-servers.title')"
					:label="$t('help.media-options.hide-offline-servers.label')"
					:text="$t('help.media-options.hide-offline-servers.text')">
					<q-toggle
						v-model="settingsStore.generalSettings.hideMediaFromOfflineServers"
						color="red" />
				</HelpRow>
				<HelpRow
					center-slot
					:title="$t('help.media-options.hide-owned-media.title')"
					:label="$t('help.media-options.hide-owned-media.label')"
					:text="$t('help.media-options.hide-owned-media.text')">
					<q-toggle
						v-model="settingsStore.generalSettings.hideMediaFromOwnedServers"
						color="red" />
				</HelpRow>
				<HelpRow
					center-slot
					:title="$t('help.media-options.use-low-quality-poster-images.title')"
					:label="$t('help.media-options.use-low-quality-poster-images.label')"
					:text="$t('help.media-options.use-low-quality-poster-images.text')">
					<q-toggle
						v-model="settingsStore.generalSettings.useLowQualityPosterImages"
						color="red" />
				</HelpRow>
			</HelpGroup>
		</template>
	</QCardDialog>
</template>

<script setup lang="ts">
import { DialogType } from '@enums';
import { useSettingsStore } from '#imports';

const settingsStore = useSettingsStore();

defineEmits<{
	(e: 'closed'): void;
}>();
</script>
