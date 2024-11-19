<template>
	<QCardDialog
		min-width="800px"
		max-width="800px"
		:name="DialogType.MediaOptionsDialog"
		:loading="false"
		close-button>
		<template #title>
			{{
				$t('components.media-options-dialog.title')
			}}
		</template>
		<!--	Help text	-->
		<template #default>
			<div>
				<QSection>
					<HelpRow
						:header-width="headerWidth"
						:title="$t('help.media-options.hide-offline-servers.title')"
						:label="$t('help.media-options.hide-offline-servers.label')"
						:text="$t('help.media-options.hide-offline-servers.text')">
						<q-toggle
							v-model="value1"
							color="red" />
					</HelpRow>
					<HelpRow
						:header-width="headerWidth"
						:title="$t('help.media-options.hide-owned-media.title')"
						:label="$t('help.media-options.hide-owned-media.label')"
						:text="$t('help.media-options.hide-owned-media.text')">
						<q-toggle
							v-model="value2"
							color="red" />
					</HelpRow>
					<HelpRow
						:header-width="headerWidth"
						:title="$t('help.media-options.use-low-quality-poster-images.title')"
						:label="$t('help.media-options.use-low-quality-poster-images.label')"
						:text="$t('help.media-options.use-low-quality-poster-images.text')">
						<q-toggle
							v-model="value3"
							color="red" />
					</HelpRow>
				</QSection>
			</div>
		</template>
	</QCardDialog>
</template>

<script setup lang="ts">
import { DialogType } from '@enums';
import HelpRow from '@components/Help/HelpRow.vue';
import { useMediaOverviewStore } from '#imports';

const headerWidth = 50;
const value1 = ref(false);
const value2 = ref(false);
const value3 = ref(false);
</script>
