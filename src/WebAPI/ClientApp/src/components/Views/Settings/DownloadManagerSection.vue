<template>
	<QSection>
		<template #header>
			{{ $t('pages.settings.advanced.download-manager.header') }}
		</template>
		<HelpGroup>
			<!--	Max segmented downloads	-->
			<HelpRow
				:label="$t('help.settings.advanced.download-manager-section.download-segments.label')"
				:title="$t('help.settings.advanced.download-manager-section.download-segments.title')"
				:text="$t('help.settings.advanced.download-manager-section.download-segments.text')">
				<QSlider
					:model-value="settingsStore.downloadManagerSettings.downloadSegments"
					label
					label-always
					:min="1"
					:max="8"
					:step="1"
					class="q-mt-lg"
					@change="settingsStore.downloadManagerSettings.downloadSegments = $event" />
			</HelpRow>
		</HelpGroup>
	</QSection>
</template>

<script setup lang="ts">
import { useSettingsStore } from '#imports';

const settingsStore = useSettingsStore();
</script>
