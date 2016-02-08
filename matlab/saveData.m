close all
clear all
clc

%Cartella dove vuoi salvare i files
destFolder = 'outData/';

%%PARAMETER SETTINGS <- YOU CAN EDIT THIS TO TRY DIFFERENT DATASET
%pick one and comment the others
datasetPath='CocktailParty'; 
%datasetPath=['CoffeeBreak' filesep 'Seq1'];
%datasetPath=['CoffeeBreak' filesep 'Seq2'];

%% DO NOT TOUCH
%loading the person positions and the detected groups
addpath(genpath('utils'));

load([datasetPath filesep 'features.mat'],'features','timestamp');
load([datasetPath filesep 'groundtruth.mat'],'GTgroups','GTtimestamp');
[~,indFeat] = intersect(timestamp,int64(GTtimestamp));
timestamp = timestamp(indFeat);
features  = features(indFeat); %evaluate only the frames having the groundtruth


%% VARIABLES EXPLAINATION
%{
the i-th features and GTgroups contains respectively the persons and the
groups in the i-th frame

features is a 1xN cells in which each cell contains [person ID, x, y, orj]
GTgroups is a 1xN cells in which each cell contains the groups in the i-th frame
%}

%% SAVE FILES

featuresID = fopen(strcat(destFolder, 'features.txt'),'w');
for f=1:numel(features)
    fprintf(featuresID, '%d %d\n', f, size(features{f},1));
    for i=1:size(features{f},1)
        fprintf(featuresID, '%d %f %f %f\n', features{f}(i,:));
    end
end
fclose(featuresID);

gtID = fopen(strcat(destFolder, 'gt.txt'),'w');
for i=1:numel(GTgroups)
    fprintf(gtID, '%d %d\n', i, numel(GTgroups{i}));
    for j=1:numel(GTgroups{i})
        for k=1:size(GTgroups{i}{j},2)
            if (k==1) 
                fprintf(gtID, '%d', GTgroups{i}{j}(1,k));
                else fprintf(gtID, ' %d', GTgroups{i}{j}(1,k));
            end
        end
        fprintf(gtID, '\n');
    end
end
fclose(gtID);
