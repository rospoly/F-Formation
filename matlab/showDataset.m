close all
clear all
clc

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

%% DETECT THE GROUPS IN FRAME f
%in this example the groups comes from the groundtruth but you have to substitute it with your detectors    
detectedGroups={};
for f=1:numel(features)   
    %something like detectGroups{f}=myGroupDetectionFunction(features{f});
    detectedGroups{f}=GTgroups{f}; 
end


%% VISUALIZER
figure;
for f=1:numel(features)      
    scatter(features{f}(:,2),features{f}(:,3)); %plot the person position (2,3 are the x,y)
    hold on;
    text(features{f}(:,2),features{f}(:,3),num2str(features{f}(:,1))); %write the person ID (1-th column)
    quiver(features{f}(:,2),features{f}(:,3),cos(features{f}(:,4)),sin(features{f}(:,4)),0.2); %draw the head/body orientation
    
    groups=detectedGroups{f}; %the groups detectectd in the f-th frame (see line 32)
    hold all %this tells matlab to change color each plot
    for g=1:numel(groups)
        idp=find(ismember(features{f}(:,1),groups{g})==1); %the persons in the g-th group
        
        %construct the container for the persons in the group using a
        %convex hull
        gfeat=[features{f}(idp,2),features{f}(idp,3)]; %get the positions from the feature set        
        gfeat=[gfeat ; gfeat-10]; %<-this is done to avoid problems in the convexhull computation
        gfeat=[gfeat ; gfeat+10]; %<-this is done to avoid problems in the convexhull computation       
        [k,~]=convhull(gfeat(:,1),gfeat(:,2)); %construct the convexhull
        plot(gfeat(k,1),gfeat(k,2)); %draw the convex hull that contains the person
    end    
    
    %% evaluate the detected groups
    [p,r] = evalgroups(groups,GTgroups(:,f)); %get the precision and recall for the detected groups
    fprintf(['Precision: ' num2str(p) ' Recall: ' num2str(r) ' F1=' num2str((2*p*r)/(p+r)) '\n']);
        
    hold off;          
    pause(1);  
end