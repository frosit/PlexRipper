<template>
	<HelpGroup v-if="plexServer">
		<HelpRow

			:label="$t('help.server-dialog.server-config.download-speed-limit.label')"
			:title="$t('help.server-dialog.server-config.download-speed-limit.title')"
			:text="$t('help.server-dialog.server-config.download-speed-limit.text')">
			<DownloadLimitInput :machine-identifier="plexServer.machineIdentifier" />
		</HelpRow>
	</HelpGroup>

	<QAlert
		v-else
		type="error">
		{{ $t('components.server-dialog.tabs.server-config.plex-server-was-null') }}
	</QAlert>
</template>

<script setup lang="ts">
import type { PlexServerDTO } from '@dto';
import HelpGroup from '@components/Help/HelpGroup.vue';

defineProps<{
	plexServer: PlexServerDTO | null;
}>();
</script>
